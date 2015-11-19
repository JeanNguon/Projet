namespace Proteca.Silverlight
{
    using Proteca.Silverlight.Resources;

    /// <summary>
    /// Wraps access to the strongly-typed resource classes so that you can bind control properties to resource strings in XAML.
    /// </summary>
    public sealed class ApplicationResources
    {
        private static readonly Resource resource = new Resource();

        private static readonly Rapports rapports = new Rapports();

        /// <summary>
        /// Gets the <see cref="ApplicationStrings"/>.
        /// </summary>
        public Resource Resource
        {
            get { return resource; }
        }

        /// <summary>
        /// Gets the <see cref="ApplicationStrings"/>.
        /// </summary>
        public Rapports Rapports
        {
            get { return rapports; }
        }
        
    }
}
