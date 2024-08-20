using System;
using System.Collections.Generic;
using System.Text;

namespace SaiphIamRolesAnywhere.DI
{
	public class RolesAnywhereExceptions : ApplicationException
	{
		public RolesAnywhereExceptions(string message)
			:
			base(message)
		{ }
		public RolesAnywhereExceptions(string message, Exception ex)
			:
			base(message, ex)
		{ }
	}
    public class CertificateNotFoundException : RolesAnywhereExceptions
    {
        public CertificateNotFoundException(string identifer)
           :
           base($"The certificate '{identifer}' was not found")
        { }
    }
    public class PathOrSubjectRequiredException : RolesAnywhereExceptions
    {
        public PathOrSubjectRequiredException()
           :
           base("A certificate path or subject must be given")
        { }
    }
}
