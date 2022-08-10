using Ar6Tcp.MessageWrappers;
using Ar6Tcp.SomethingSend;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ar6Tcp.Client
{
	public class Ar6TcpClient
	{
		readonly private static byte[] _OKCode = Encoding.UTF8.GetBytes("OK");
		public TcpClient _TcpClient { get; private set; }
		public NetworkStream NetStream { get; private set; }
		public delegate void DataReceivedHandler(SomethingToSend dataForSendOrOnline);
		public event DataReceivedHandler DataReceivedEvent;
		public IPEndPoint IPPort;
		int _ID;
		string _Name;
		public OnlineInfo onlineInfo;
		public Ar6TcpClient(IPEndPoint IPPort)
		{
			_TcpClient = new TcpClient();
			this.IPPort = IPPort;
		}
		public bool Connect(IPEndPoint IPPort)
		{
			if (_TcpClient.Connected)
				return true;
			try
			{
				_TcpClient.Connect(IPPort);
				NetStream = _TcpClient.GetStream();
				WhileIsConnected();
				WhileReadData();
				return true;
			}
			catch (Exception ex) when(ex is SocketException || ex is InvalidOperationException)
			{
				if (ex is InvalidOperationException)
				{
					_TcpClient.Close();
					_TcpClient = new TcpClient();
				}
				return false;
			}
		}
		async void WhileReadData()
		{
			await Task.Run(() =>
			{
				while (_TcpClient.Connected)
				{
					ReadData();
					Thread.Sleep(1);
				}
			});
		}
		async void WhileIsConnected()
		{
			await Task.Run(() =>
			{
				bool iConnected;
				do
				{
					iConnected = IsConnected();
					Thread.Sleep(100);
				} while (iConnected);
			});
		}
		public void Disconnect()
		{
			_TcpClient.Close();
		}
		async public Task<bool> GetOnlineAsync()
		{
			return await Task.Run(() =>
			{
				return SendData(new OnlineInfo());
			});
		}
		public void SendData(string data, int ID)
		{
			SendData(new DataForSend() { Data = data, EncodingName = Encoding.UTF8.WebName, ID = ID, Length = data.Length, SenderID = this._ID, SenderName = _Name});
		}
		object o = new object();
		bool _canRead = true;
		/// <param name="ID">ID to whom to send</param>
		private bool SendData(SomethingToSend somethingToSend)
		{
			lock (o)
			{
				if (somethingToSend is DataForSend dfs)
				{
					return SendDFS(dfs);
				}
				else if (somethingToSend is OnlineInfo oi)
				{
					return SendGetOnline(oi);
				}
				else return false;
			}
		}
		bool SendGetOnline(OnlineInfo oi)
		{
			_canRead = false;
			int lastTimeout = NetStream.ReadTimeout;
			//NetStream.ReadTimeout = 5000;
			try
			{
				var messageWrapper = new MessageWrapper<OnlineInfo>() { Message = oi };
				string jsonStr = JsonConvert.SerializeObject(messageWrapper);
				byte[] buffer = Encoding.UTF8.GetBytes(jsonStr);
				byte[] bufferLength = Encoding.UTF8.GetBytes(buffer.Length.ToString());
				byte[] bufferOK = new byte[_OKCode.Length];
				NetStream.Write(bufferLength, 0, bufferLength.Length);
				NetStream.Read(bufferOK, 0, bufferOK.Length);
				if (!bufferOK.SequenceEqual(_OKCode))
				{
					_canRead = true;
					return false;
				}
				NetStream.Write(buffer, 0, buffer.Length);
				//
				NetStream.Read(bufferLength, 0, bufferLength.Length);
				NetStream.Write(_OKCode, 0, _OKCode.Length);
				
				int.TryParse(Encoding.UTF8.GetString(bufferLength), out int length);
				byte[] bufferToRead = new byte[length];
				int countReaded = 0;
				do
				{
					countReaded += NetStream.Read(bufferToRead, countReaded, bufferToRead.Length);
				} while (countReaded != length);

				string str = Encoding.UTF8.GetString(bufferToRead);
				object ooi = MessageWrapper.Deserialize(str).Item2;
				if (!(ooi is OnlineInfo readedOI))
					return false;
				onlineInfo = readedOI;

				if (onlineInfo != null)
				{
					_canRead = true;
					return true;
				}

				_canRead = true;
				return false;
			}
			catch (Exception)
			{
				NetStream.ReadTimeout = lastTimeout;
				return false;
			}
			finally
			{
				_canRead = true;
			}
		}
		bool SendDFS(DataForSend dfs)
		{
			_canRead = false;
			var messageWrapper = new MessageWrapper<DataForSend>() { Message = dfs.GetDataForSendWithoutData() };
			string dfsStrWithoutData = JsonConvert.SerializeObject(messageWrapper);

			byte[] buffer = Encoding.UTF8.GetBytes(dfsStrWithoutData);
			byte[] bufferLength = Encoding.UTF8.GetBytes(buffer.Length.ToString());

			byte[] dataSend = Encoding.GetEncoding(dfs.EncodingName).GetBytes(dfs.Data);

			NetStream.Write(bufferLength, 0, bufferLength.Length);
			byte[] bufferOK = new byte[2];
			NetStream.Read(bufferOK, 0, 2);
			if (bufferOK.SequenceEqual(_OKCode))
				NetStream.Write(buffer, 0, buffer.Length);
			else { _canRead = true; return false; }

			bufferOK = new byte[2];
			NetStream.Read(bufferOK, 0, 2);
			if (bufferOK.SequenceEqual(_OKCode))
				NetStream.Write(dataSend, 0, dataSend.Length);
			else { _canRead = true; return false; }
			_canRead = true;
			return true;
		}
		public bool IsConnected()
		{
			try
			{
				NetStream.Write(new byte[1], 0, 0);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}
		void ReadData()
		{
			lock (o)
			{
				if (!NetStream.DataAvailable || !_canRead)
					return;

				byte[] buffer = new byte[256];
				int countByte;
				countByte = NetStream.Read(buffer, 0, buffer.Length);
				string lenghtOrOnlineInfo = Encoding.UTF8.GetString(buffer, 0, countByte);
				if (int.TryParse(lenghtOrOnlineInfo, out int length))
					NetStream.Write(_OKCode, 0, _OKCode.Length);
				
				int countReaded = 0;
				buffer = new byte[length];
				do
				{
					countByte = NetStream.Read(buffer, countReaded, buffer.Length);
					countReaded += countByte;
				} while (countReaded != length);
				string JsonWithoutData = Encoding.UTF8.GetString(buffer, 0, countByte);

				var messageReaded = JsonConvert.DeserializeObject<MessageWrapper>(JsonWithoutData);
				DataForSend dfs = new DataForSend();
				NetStream.Write(_OKCode, 0, _OKCode.Length);

				countReaded = 0;
				buffer = new byte[dfs.Length];
				do
				{
					countByte = NetStream.Read(buffer, countReaded, buffer.Length);
					countReaded += countByte;
				} while (countReaded != dfs.Length);
				dfs.Data = Encoding.GetEncoding(dfs.EncodingName).GetString(buffer, 0, countByte);
				DataReceivedEvent?.Invoke(dfs);
			}
		}
	}
}
