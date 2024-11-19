using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Text;

namespace WebAPIServiceTemplate1.Business.PGP
{
  public static class PgpByteArray
  {
    private const int BufferSize = 512;

    public static PgpPublicKey ReadPublicKey(Stream inputStream)
    {
      inputStream = PgpUtilities.GetDecoderStream(inputStream);
      foreach (PgpPublicKeyRing keyRing in new PgpPublicKeyRingBundle(inputStream).GetKeyRings())
      {
        foreach (PgpPublicKey publicKey in keyRing.GetPublicKeys())
        {
          if (publicKey.IsEncryptionKey)
            return publicKey;
        }
      }
      throw new ArgumentException("Can't find encryption key in key ring.");
    }

    private static PgpPrivateKey FindSecretKey(
      PgpSecretKeyRingBundle pgpSec,
      long keyId,
      char[] pass)
    {
      return pgpSec.GetSecretKey(keyId)?.ExtractPrivateKey(pass);
    }

    public static byte[] Decrypt(byte[] inputData, Stream keyIn, string passCode)
    {
      byte[] bytes = Encoding.ASCII.GetBytes("ERROR");
      Stream decoderStream = PgpUtilities.GetDecoderStream((Stream) new MemoryStream(inputData));
      MemoryStream memoryStream = new MemoryStream();
      try
      {
        PgpObjectFactory pgpObjectFactory = new PgpObjectFactory(decoderStream);
        PgpObject pgpObject1 = pgpObjectFactory.NextPgpObject();
        PgpEncryptedDataList encryptedDataList = !(pgpObject1 is PgpEncryptedDataList) ? (PgpEncryptedDataList) pgpObjectFactory.NextPgpObject() : (PgpEncryptedDataList) pgpObject1;
        PgpPrivateKey privKey = (PgpPrivateKey) null;
        PgpPublicKeyEncryptedData keyEncryptedData = (PgpPublicKeyEncryptedData) null;
        PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));
        foreach (PgpPublicKeyEncryptedData encryptedDataObject in encryptedDataList.GetEncryptedDataObjects())
        {
          privKey = PgpByteArray.FindSecretKey(pgpSec, encryptedDataObject.KeyId, passCode.ToCharArray());
          if (privKey != null)
          {
            keyEncryptedData = encryptedDataObject;
            break;
          }
        }
        if (privKey == null)
          throw new ArgumentException("secret key for message not found.");
        PgpObject pgpObject2 = new PgpObjectFactory(keyEncryptedData.GetDataStream(privKey)).NextPgpObject();
        if (pgpObject2 is PgpCompressedData)
          pgpObject2 = new PgpObjectFactory(((PgpCompressedData) pgpObject2).GetDataStream()).NextPgpObject();
        if (pgpObject2 is PgpLiteralData)
        {
          PgpByteArray.PipeAll(((PgpLiteralData) pgpObject2).GetInputStream(), (Stream) memoryStream);
          keyEncryptedData.IsIntegrityProtected();
          return memoryStream.ToArray();
        }
        if (pgpObject2 is PgpOnePassSignatureList)
          throw new PgpException("encrypted message contains a signed message - not literal data.");
        throw new PgpException("message is not a simple encrypted file - type unknown.");
      }
      catch (Exception ex)
      {
        return bytes;
      }
    }

    public static byte[] Encrypt(
      byte[] inputData,
      PgpPublicKey passPhrase,
      bool withIntegrityCheck,
      bool armor)
    {
      byte[] buffer = PgpByteArray.Compress(inputData, "_CONSOLE", CompressionAlgorithmTag.Uncompressed);
      MemoryStream memoryStream = new MemoryStream();
      Stream stream1 = (Stream) memoryStream;
      if (armor)
        stream1 = (Stream) new ArmoredOutputStream(stream1);
      PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
      encryptedDataGenerator.AddMethod(passPhrase);
      Stream stream2 = encryptedDataGenerator.Open(stream1, (long) buffer.Length);
      stream2.Write(buffer, 0, buffer.Length);
      stream2.Close();
      if (armor)
        stream1.Close();
      return memoryStream.ToArray();
    }

    public static byte[] Encrypt(byte[] inputData, byte[] publicKey)
    {
      PgpPublicKey passPhrase = PgpByteArray.ReadPublicKey((Stream) new MemoryStream(publicKey));
      return PgpByteArray.Encrypt(inputData, passPhrase, true, true);
    }

    private static byte[] Compress(
      byte[] clearData,
      string fileName,
      CompressionAlgorithmTag algorithm)
    {
      MemoryStream memoryStream = new MemoryStream();
      PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(algorithm);
      Stream stream = new PgpLiteralDataGenerator().Open(compressedDataGenerator.Open((Stream) memoryStream), 'b', fileName, (long) clearData.Length, DateTime.UtcNow);
      stream.Write(clearData, 0, clearData.Length);
      stream.Close();
      compressedDataGenerator.Close();
      return memoryStream.ToArray();
    }

    public static void PipeAll(Stream inStr, Stream outStr)
    {
      byte[] buffer = new byte[512];
      int count;
      while ((count = inStr.Read(buffer, 0, buffer.Length)) > 0)
        outStr.Write(buffer, 0, count);
    }
  }
}
