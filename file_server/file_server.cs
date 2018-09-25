using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace tcp
{
	class file_server
	{
       
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
 		/// </summary>
		private file_server ()
		{
			
            Socket ourSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ourSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
		    ourSocket.Listen(0);

  
            // Here we block until a connection is made (we are here waiting for our host)
		    Socket acceptSocket = ourSocket.Accept();

		    Console.WriteLine("Someone connected!");

		    NetworkStream myNetworkStream = new NetworkStream(acceptSocket);

		    while (true)
		    {
		        string ReceivedFilePath = LIB.readTextTCP(myNetworkStream);

		        long fileSize = LIB.check_File_Exists(ReceivedFilePath);
		        Console.WriteLine($"Received file path: {ReceivedFilePath}");
		        Console.WriteLine($"Filesize: {fileSize}");

		        if (fileSize == 0)
		        {
		            Console.WriteLine("Could not find file.");
		            LIB.writeTextTCP(myNetworkStream, "Error: Could not find file.");
		        }
		        else
		        {
		            sendFile(ReceivedFilePath, fileSize, myNetworkStream);
		        }
            }
            
            ourSocket.Close();
            acceptSocket.Close();
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String filePath, long fileSize, NetworkStream io)
		{
		    LIB.writeTextTCP(io, fileSize.ToString());

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		    byte[] fileBuf = System.IO.File.ReadAllBytes(filePath);

		    int offset = 0;
		    int size = 1000;

		    while (offset < fileBuf.Length)
		    {
		        io.Write(fileBuf, offset, size);

		        offset += 1000;

		        if ((offset < fileBuf.Length) && (offset + 1000 > fileBuf.Length))
		        {
		            size = fileBuf.Length - offset;
		        }
		    }

        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		///
		/// 
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new file_server();
		    System.Console.ReadKey();
        }
	}
}
