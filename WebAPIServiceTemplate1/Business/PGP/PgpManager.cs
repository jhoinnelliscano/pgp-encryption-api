using System.IO;

namespace WebAPIServiceTemplate1.Business.PGP
{
  public static class PgpManager
  {
    public static void GenerateKeys(
      string clientPath,
      string publicKey,
      string privateKey,
      string userName,
      string password,
      int keyLength)
    {
      using (PgpCore.PGP pgp = new PgpCore.PGP())
      {
        if (!Directory.Exists(clientPath))
          Directory.CreateDirectory(clientPath);
        pgp.GenerateKey(publicKey, privateKey, userName, password, keyLength, 8);
      }
    }

    public static void Decrypt(
      string fileName,
      string keyFileName,
      char[] password,
      string fileNameDrecripted)
    {
      PgpFile.DecryptFile(fileName, keyFileName, password, fileNameDrecripted);
    }

    public static byte[] Decrypt(byte[] data, Stream keyIn, string password)
    {
      return PgpByteArray.Decrypt(data, keyIn, password);
    }

    public static void Encrypt(
      string fileNameEncryted,
      string fileName,
      string keyFileName,
      bool armor,
      bool withIntegrityCheck)
    {
      PgpFile.EncryptFile(fileNameEncryted, fileName, keyFileName, armor, withIntegrityCheck);
    }

    public static byte[] Encrypt(byte[] data, byte[] publicKey)
    {
      return PgpByteArray.Encrypt(data, publicKey);
    }
  }
}
