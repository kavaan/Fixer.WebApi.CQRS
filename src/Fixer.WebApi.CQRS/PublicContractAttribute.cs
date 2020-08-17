using System;
using System.Collections.Generic;
using System.Text;

namespace Fixer.WebApi.CQRS
{
    //Marker
    [AttributeUsage(AttributeTargets.Class)]
    public class PublicContractAttribute : Attribute
    {
    }
}
