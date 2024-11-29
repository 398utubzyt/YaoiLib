namespace YaoiLib
{
    /// <summary>
    /// A collection of primitive constants casted to <see cref="object"/>.
    /// </summary>
    public static class BoxedPrimitives
    {
        public static readonly object True = true;
        public static readonly object False = false;

        public static readonly object Zero = 0;
        public static readonly object One = 1;
        public static readonly object MinusOne = -1;

        public static object Bool(bool value) => value ? True : False;
    }
}
