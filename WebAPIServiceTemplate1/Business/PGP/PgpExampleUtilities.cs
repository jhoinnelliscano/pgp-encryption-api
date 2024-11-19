using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;

namespace WebAPIServiceTemplate1.Business.PGP
{
  public class PgpExampleUtilities
  {
    internal static PgpPrivateKey FindSecretKey(
      PgpSecretKeyRingBundle pgpSec,
      long keyID,
      char[] pass)
    {
      return pgpSec.GetSecretKey(keyID)?.ExtractPrivateKey(pass);
    }

    internal static PgpPublicKey ReadPublicKey(string fileName)
    {
      using (Stream input = (Stream) File.OpenRead(fileName))
        return PgpExampleUtilities.ReadPublicKey(input);
    }

    internal static PgpPublicKey ReadPublicKey(Stream input)
    {
      foreach (PgpPublicKeyRing keyRing in new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(input)).GetKeyRings())
      {
        foreach (PgpPublicKey publicKey in keyRing.GetPublicKeys())
        {
          if (publicKey.IsEncryptionKey)
            return publicKey;
        }
      }
      throw new ArgumentException("Can't find encryption key in key ring.");
    }

    internal static byte[] CompressFile(string fileName, CompressionAlgorithmTag algorithm)
    {
      MemoryStream memoryStream = new MemoryStream();
      PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(algorithm);
      PgpUtilities.WriteFileToLiteralData(compressedDataGenerator.Open((Stream) memoryStream), 'b', new FileInfo(fileName));
      compressedDataGenerator.Close();
      return memoryStream.ToArray();
    }
  }
}
