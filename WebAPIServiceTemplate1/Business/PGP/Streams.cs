using System.IO;

namespace WebAPIServiceTemplate1.Business.PGP
{
  public class Streams
  {
    private const int BufferSize = 512;

    public static void PipeAll(Stream inStr, Stream outStr)
    {
      byte[] buffer = new byte[512];
      int count;
      while ((count = inStr.Read(buffer, 0, buffer.Length)) > 0)
        outStr.Write(buffer, 0, count);
    }
  }
}
