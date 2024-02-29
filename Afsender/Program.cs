using System.Security.Cryptography;
using Reciever.Decrypter;
using System.Text;
using System.Diagnostics;

namespace Reciever
{
	internal class Program
	{
		static bool projectLaunched = false;
		static string KeyContainerName = "RSAKey";
		static void Main(string[] args)
		{
			try
			{


				if (!DoesKeyContainerExist(KeyContainerName))
				{
					Console.WriteLine("key generated.");

					//first we generate a key if it does not exist.
					using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
					{
						CspParameters cspParams = new CspParameters() { KeyContainerName = KeyContainerName };
						rsa.PersistKeyInCsp = true;
						rsa.ImportParameters(rsa.ExportParameters(false));
					}
				}


				// start the other project. (currently just keeps starting instances of this project, not the other one...
				Console.WriteLine("Reciever");
				//if (!projectLaunched)
				//{
				//	projectLaunched = true;
				//	string projectPath = "chrome.exe"; //test
				//	Process.Start(projectPath);
				//}

				//Create a UnicodeEncoder to convert between byte array and string.
				UnicodeEncoding byteConverter = new UnicodeEncoding();

				//Create a new instance of RSACryptoServiceProvider to generate
				//public and private key data.
				using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024, new CspParameters() { KeyContainerName = KeyContainerName }))
				{
					//export the public key to export the modulus and exponent.
					RSAParameters publicKey = rsa.ExportParameters(true);




					//display the exponent and modulus used.
					Console.WriteLine("public data:");
					Console.WriteLine("modulus: {0}", BitConverter.ToString(publicKey.Modulus));
					Console.WriteLine("exponent: {0}\n\n", BitConverter.ToString(publicKey.Exponent));

					Console.WriteLine("press a key to continue...");
					Console.ReadKey();
					Console.WriteLine("\nWrite message to decrypt:");
					string message = Console.ReadLine();



					// Remove the dashes and split the string into an array of hexadecimal strings
					string[] hexPairs = message.Split('-');

					// Convert each hexadecimal string to a byte
					byte[] modulusBytes = new byte[hexPairs.Length];
					for (int i = 0; i < hexPairs.Length; i++)
					{
						modulusBytes[i] = Convert.ToByte(hexPairs[i], 16);
					}


					//Create byte arrays to hold original, encrypted, and decrypted data.
					byte[] dataToDecrypt = modulusBytes;
					byte[] decryptedData;

					Console.WriteLine("Private Data");
					Console.WriteLine("D: {0}", BitConverter.ToString(publicKey.D));
					Console.WriteLine("DP: {0}", BitConverter.ToString(publicKey.DP));
					Console.WriteLine("DQ: {0}", BitConverter.ToString(publicKey.DQ));
					Console.WriteLine("Inverse Q: {0}", BitConverter.ToString(publicKey.InverseQ));
					Console.WriteLine("P: {0}", BitConverter.ToString(publicKey.P));
					Console.WriteLine("Q: {0}", BitConverter.ToString(publicKey.Q));

					//Pass the data to Decrypt, the public key information 
					//(using RSACryptoServiceProvider.ExportParameters(false),
					//and a boolean flag specifying no OAEP padding.
					decryptedData = RSADecrypter.RSADecrypt(dataToDecrypt, rsa.ExportParameters(true), false);
					//Display the decrypted plaintext and the encrypted bytes to the console. 
					Console.WriteLine("Cipherbytes: {0}", BitConverter.ToString(modulusBytes));
					Console.WriteLine("Decrypted: \n{0}", byteConverter.GetString(decryptedData));
				}
			}
			catch (ArgumentNullException)
			{
				//Catch this exception in case the encryption did
				//not succeed.
				Console.WriteLine("Decryption failed.");
			}
			Console.Read();
		}

		static bool DoesKeyContainerExist(string containerName)
		{
			CspParameters cspParams = new CspParameters() { KeyContainerName = containerName };
			try
			{
				// Try to access the key container
				using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams))
				{
					rsa.PersistKeyInCsp = false;
					// If accessing the container is successful, it exists
					return true;
				}
			}
			catch (Exception)
			{
				// If accessing the container throws an exception, it does not exist
				return false;
			}
		}
	}
}
