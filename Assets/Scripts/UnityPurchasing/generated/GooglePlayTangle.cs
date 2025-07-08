// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("41HS8ePe1dr5VZtVJN7S0tLW09BR0tzT41HS2dFR0tLTQMV2bTAJ90In9HLAs/dTgyQGSAXssw8Pok2XvoW+l4eo5eexuzV7qKr0t1WjdtBEhkvT7QsdicjbiQEOpUJ3kN5mXaEsd68E8O4nXTFe3efH82loZT+BMtskhtfxLHOnYg4rVRXYdlJ6z8IEto+iGtSTf4xh1AFVpEtscY8qEResAoyvn7sZYJG7ICQZM2Zrtt04nCLGFD6QNZvt/mFIMhgXaglI78S13C7LVXon43DyuO/E2HGqAneKOTwQfdTcLlb57EzPYi/YJfQ+OJ1CPBAZyewReZn/vN3FhFsxQFlKYYKQpu/DtRP1Th8Bm2QaZSFzHlPOu7pMNy4uz4gzLtHQ0tPS");
        private static int[] order = new int[] { 0,1,8,8,6,6,13,7,10,9,13,11,12,13,14 };
        private static int key = 211;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
