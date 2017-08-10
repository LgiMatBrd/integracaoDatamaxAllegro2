using System.Globalization;
using System.Text;

namespace System
{
  public static class StringExtensions
  {
    public static string PadBoth(this string str, int length)
    {
      int int32 = Convert.ToInt32((double) (length - str.Length) * 1.4);
      return str.ToUpper().PadLeft(int32, ' ').PadRight(int32 * 2, ' ');
    }

    public static string RemoveAccents(this string text)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in text.Normalize(NormalizationForm.FormD).ToCharArray())
      {
        if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }
  }
}
