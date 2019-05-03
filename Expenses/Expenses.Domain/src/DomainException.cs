using System;

namespace Expenses.Domain {
    public class DomainException : Exception {
        public DomainException(DomainExceptionCause cause, string message)
            : base(message) {
            
            Cause = cause;
        }
        public DomainExceptionCause Cause { get; private set; }        
    }
}