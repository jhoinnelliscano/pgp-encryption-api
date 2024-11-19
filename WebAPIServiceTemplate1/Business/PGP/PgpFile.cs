using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace WebAPIServiceTemplate1.Business.PGP
{
  public class PgpFile
  {
    public static void DecryptFile(
      string inputFileName,
      string keyFileName,
      char[] passwd,
      string defaultFileName)
    {
      using (Stream inputStream = (Stream) File.OpenRead(inputFileName))
      {
        using (Stream keyIn = (Stream) File.OpenRead(keyFileName))
          PgpFile.DecryptFile(inputStream, keyIn, passwd, defaultFileName);
      }
    }

    private static void DecryptFile(
      Stream inputStream,
      Stream keyIn,
      char[] passwd,
      string defaultFileName)
    {
      inputStream = PgpUtilities.GetDecoderStream(inputStream);
      try
      {
        PgpObjectFactory pgpObjectFactory = new PgpObjectFactory(inputStream);
        PgpObject pgpObject1 = pgpObjectFactory.NextPgpObject();
        PgpEncryptedDataList encryptedDataList = !(pgpObject1 is PgpEncryptedDataList) ? (PgpEncryptedDataList) pgpObjectFactory.NextPgpObject() : (PgpEncryptedDataList) pgpObject1;
        PgpPrivateKey privKey = (PgpPrivateKey) null;
        PgpPublicKeyEncryptedData keyEncryptedData = (PgpPublicKeyEncryptedData) null;
        PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));
        foreach (PgpPublicKeyEncryptedData encryptedDataObject in encryptedDataList.GetEncryptedDataObjects())
        {
          privKey = PgpExampleUtilities.FindSecretKey(pgpSec, encryptedDataObject.KeyId, passwd);
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
          PgpLiteralData pgpLiteralData = (PgpLiteralData) pgpObject2;
          string path = pgpLiteralData.FileName;
          if (path.Length == 0)
            path = defaultFileName;
          Stream outStr = (Stream) File.Create(path);
          Streams.PipeAll(pgpLiteralData.GetInputStream(), outStr);
          outStr.Close();
          if (keyEncryptedData.IsIntegrityProtected())
          {
            if (!keyEncryptedData.Verify())
              Console.Error.WriteLine("message failed integrity check");
            else
              Console.Error.WriteLine("message integrity check passed");
          }
          else
            Console.Error.WriteLine("no message integrity check");
        }
        else
        {
          if (pgpObject2 is PgpOnePassSignatureList)
            throw new PgpException("encrypted message contains a signed message - not literal data.");
          throw new PgpException("message is not a simple encrypted file - type unknown.");
        }
      }
      catch (PgpException ex)
      {
        Console.Error.WriteLine((object) ex);
        Exception innerException = ex.InnerException;
        if (innerException == null)
          return;
        Console.Error.WriteLine(innerException.Message);
        Console.Error.WriteLine(innerException.StackTrace);
      }
    }

    public static void EncryptFile(
      string outputFileName,
      string inputFileName,
      string encKeyFileName,
      bool armor,
      bool withIntegrityCheck)
    {
      PgpPublicKey encKey = PgpExampleUtilities.ReadPublicKey(encKeyFileName);
      using (Stream outputStream = (Stream) File.Create(outputFileName))
        PgpFile.EncryptFile(outputStream, inputFileName, encKey, armor, withIntegrityCheck);
    }

    private static void EncryptFile(
      Stream outputStream,
      string fileName,
      PgpPublicKey encKey,
      bool armor,
      bool withIntegrityCheck)
    {
      if (armor)
        outputStream = (Stream) new ArmoredOutputStream(outputStream);
      try
      {
        byte[] buffer = PgpExampleUtilities.CompressFile(fileName, CompressionAlgorithmTag.Zip);
        PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
        encryptedDataGenerator.AddMethod(encKey);
        Stream stream = encryptedDataGenerator.Open(outputStream, (long) buffer.Length);
        stream.Write(buffer, 0, buffer.Length);
        stream.Close();
        if (!armor)
          return;
        outputStream.Close();
      }
      catch (PgpException ex)
      {
        Console.Error.WriteLine((object) ex);
        Exception innerException = ex.InnerException;
        if (innerException == null)
          return;
        Console.Error.WriteLine(innerException.Message);
        Console.Error.WriteLine(innerException.StackTrace);
      }
    }
  }
}
