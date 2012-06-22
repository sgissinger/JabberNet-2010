using System.Collections;

namespace NetLib.DNS
{
    /// <summary>
    /// Represents a collection of <see cref="DnsWrapper"/> objects.
    /// </summary>
    /// <remarks>
    /// The DnsWrapperCollection is a collection of <see cref="DnsWrapper"/>
    /// objects. The resultant collection represents all of the DNS records
    /// for the given domain that was looked up. This class cannot be directly
    /// created - it is created by the <see cref="DnsRequest"/> and
    /// <see cref="DnsResponse"/> classes to hold the returned DNS
    /// records for the given domain.
    /// </remarks>
    public class DnsWrapperCollection : ReadOnlyCollectionBase, IEnumerable
    {
        internal DnsWrapperCollection()
        {
        }

        internal bool Contains(DnsWrapper w)
        {
            foreach (DnsWrapper wrapper in InnerList)
                if (w.Equals(wrapper))
                    return true;

            return false;
        }

        internal void Add(DnsWrapper w)
        {
            InnerList.Add(w);
        }

        /// <summary>
        /// Gets the <see cref="DnsWrapper"/> at the specified
        /// ordinal in the collection
        /// </summary>
        /// <remarks>
        /// Gets the <see cref="DnsWrapper"/> at the specified
        /// index of the collection.
        /// </remarks>
        /// <param name="i">The index to retrieve from the collection.</param>
        /// <value>The <see cref="DnsWrapper"/> at the specified index of
        /// the collection.</value>
        public DnsWrapper this[int i]
        {
            get
            {
                return (DnsWrapper)InnerList[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DnsWrapperCollectionEnumerator(this);
        }

        class DnsWrapperCollectionEnumerator : IEnumerator
        {
            private int idx = -1;
            private readonly DnsWrapperCollection coll;

            public DnsWrapperCollectionEnumerator(DnsWrapperCollection coll)
            {
                this.coll = coll;
            }

            void IEnumerator.Reset()
            {
                idx = -1;
            }

            bool IEnumerator.MoveNext()
            {
                idx++;

                return idx < coll.Count;
            }

            object IEnumerator.Current
            {
                get
                {
                    return coll[idx];
                }
            }
        }
    }
}
