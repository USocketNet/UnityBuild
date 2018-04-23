using System;
using System.IO;
using System.IO.Compression;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

namespace BytesCrafter.USocketNet
{
	public class USocket
	{
		private static USocket udata = null;
		public static USocket Instance
		{
			get
			{
				if(udata == null)
				{
					udata = new USocket ();
				}

				return udata;
			}
		}

		public int synchRate = 0;
		public float uploadRate = 0f;
		public float downloadRate = 0f;

		/// <summary>
		/// The usocket nets client in this local machine.
		/// </summary>
		public List<USocketClient> usocketNets = new List<USocketClient>();

		/// <summary>
		/// The socket view of all the other machine's.
		/// </summary>
		public List<USocketView> socketIdentities = new List<USocketView>();
	}
}

namespace BytesCrafter.USocketNet.Compression
{
	internal static class GZip
	{
		public static string Compress(string dataString)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(dataString);
			var memoryStream = new MemoryStream();
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
			{
				gZipStream.Write(buffer, 0, buffer.Length);
			}

			memoryStream.Position = 0;

			var compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);

			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}

		public static string Decompress(string compressedString)
		{
			byte[] gZipBuffer = Convert.FromBase64String(compressedString);
			using (var memoryStream = new MemoryStream())
			{
				int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
				memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

				var buffer = new byte[dataLength];

				memoryStream.Position = 0;
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					gZipStream.Read(buffer, 0, buffer.Length);
				}

				return Encoding.UTF8.GetString(buffer);
			}
		}
	}
}