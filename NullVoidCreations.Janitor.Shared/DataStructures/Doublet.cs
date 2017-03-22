
namespace NullVoidCreations.Janitor.Shared.DataStructures
{
    public class Doublet<F, S>
    {
        public Doublet(F first, S second)
        {
            First = first;
            Second = second;
        }

        #region properties

        public F First { get; set; }

        public S Second { get; set; }

        #endregion

        public static Doublet<F, S> Create(F first, S second)
        {
            return new Doublet<F, S>(first, second);
        }
    }
}
