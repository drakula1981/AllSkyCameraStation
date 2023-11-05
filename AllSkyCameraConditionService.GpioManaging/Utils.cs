namespace AllSkyCameraConditionService.GpioManaging;
internal static class Utils {
   public static string Ordinal(this int number) {
      const string TH = "th";
      string s = number.ToString();

      // Negative and zero have no ordinal representation
      if (number < 1) return s;

      number %= 100;
      if ((number >= 11) && (number <= 13)) return s + TH;

      return (number % 10) switch {
         1 => s + "st",
         2 => s + "nd",
         3 => s + "rd",
         _ => s + TH,
      };
   }
}