using System;

namespace DragonDrop.Infrastructure.Helpers
{
    public static class BarcodeVerifier
    {
        public static bool Check(string barCode)
        {
            var oddSum = 0;
            var evenSum = 0;

            for (int i = 0; i < 11; i++)
            {
                var digit = 0;
                try
                {
                    digit = int.Parse(barCode[i].ToString());
                }
                catch (Exception)
                {
                    return false;
                }

                oddSum += i % 2 == 0 ? digit: 0;
                evenSum += i % 2 != 0 ? digit: 0;
            }

            var totalSum = oddSum * 3 + evenSum;

            var check = 0;
            while (true)
            {
                if ((check + totalSum) % 10 == 0) break;
                check++;
            }

            return check == int.Parse(barCode.Substring(11, 1));
        }
    }
}
