using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projector.ObjectModel.TypeModel
{
    class ProjectionDeclaredPropertyInitialization<T> : ITraitAggregator
    {
        private ProjectionDeclaredProperty<T> projectionDeclaredProperty;
        private ProjectionPropertyCollection properties;

        public ProjectionDeclaredPropertyInitialization(ProjectionDeclaredProperty<T> projectionDeclaredProperty, ProjectionPropertyCollection properties)
        {
            // TODO: Complete member initialization
            this.projectionDeclaredProperty = projectionDeclaredProperty;
            this.properties = properties;
        }

        public void Add(object trait)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty> Overrides { get; set; }
    }
}
