namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class ProjectionPropertySite
    {
        private readonly ProjectionProperty property;
        private Func  <Projection, object>  getter;
        private Action<Projection, object>  setter;
        private Action<Projection>          invalidator;

        public ProjectionPropertySite()
        {

        }
    }
}
