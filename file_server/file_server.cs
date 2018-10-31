using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace tcp
{
	class file_server
	{
		const int PORT = 9000;

		private file_server ()
		{
			
            Socket ourSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ourSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));

		    while (true)
		    {
				ourSocket.Listen(0);

                Socket acceptSocket = ourSocket.Accept();

                Console.WriteLine("Someone connected!");

                NetworkStream myNetworkStream = new NetworkStream(acceptSocket);

				string ReceivedFilePath = LIB.readTextTCP(myNetworkStream);

		        long fileSize = LIB.check_File_Exists(ReceivedFilePath);
		        Console.WriteLine($"Received file path: {ReceivedFilePath}");
		        Console.WriteLine($"Filesize: {fileSize} bytes");

		        if (fileSize == 0)
		        {
		            Console.WriteLine("Could not find file.");
		            LIB.writeTextTCP(myNetworkStream, "Error: Could not find file.");
		        }
		        else
		        {
		            sendFile(ReceivedFilePath, fileSize, myNetworkStream);
		        }

				acceptSocket.Close();
            }
            
            //ourSocket.Close();
            
		}

		private void sendFile (String filePath, long fileSize, NetworkStream io)
		{
		    LIB.writeTextTCP(io, fileSize.ToString());

            long length = new System.IO.FileInfo(filePath).Length;
            FileStream fs = new FileStream(filePath, FileMode.Open);

		    byte[] fileBuf = new byte[1000];

            int offset = 0;
		    int size = 1000;

            while (offset < length)
		    {
		        fs.Read(fileBuf, 0, size);
                
                io.Write(fileBuf, 0, size);

		        offset += 1000;

		        if ((offset < length) && (offset + 1000 > length))
		        {
		            size = (int)length - offset;
		        }
		    }

			fs.Close();
        }

		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			file_server fs1 = new file_server();
        }
	}
}
