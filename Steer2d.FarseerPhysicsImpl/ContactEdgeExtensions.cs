using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarseerPhysics.Dynamics.Contacts
{
    public static class ContactEdgeExtensions
    {
        public static IEnumerable<ContactEdge> GetContactEdges(this ContactEdge first)
        {
            IList<ContactEdge> result = new List<ContactEdge>();
            var contactEdge = first;

            while (contactEdge != null)
            {
                result.Add(contactEdge);
                contactEdge = contactEdge.Next;
            }

            return result;
        }
    }
}
