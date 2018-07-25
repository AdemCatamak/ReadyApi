using System;
using NUnit.Framework;
using ReadyApi.CustomExceptions;

namespace ReadyApi.UnitTest.CustomExceptionTest
{
    
    public class FriendlyExceptionTest
    {
        [Test]
        public void Alternatives_UnitTest_CustomExceptionTest__FriendlyExceptionCreation_WithMessage()
        {
            FriendlyException friendlyException = new FriendlyException("Friendly Message");

            Assert.IsNull(friendlyException.InnerException);
            Assert.IsNotNull(friendlyException.FriendlyMessage);
        }

        [Test]
        public void Alternatives_UnitTest_CustomExceptionTest__FriendlyExceptionCreation_WithMessageAndException()
        {
            Exception ex = new Exception("Common Exception");
            FriendlyException friendlyException = new FriendlyException("Friendly Message", ex);

            Assert.IsNotNull(friendlyException.InnerException , "FriendlyException's inner exception is null");
            Assert.IsFalse(string.IsNullOrEmpty(friendlyException.FriendlyMessage), "FriendlyException's friendly message is null or empty");
            Assert.AreEqual(ex.Message, friendlyException.InnerException.Message , "Friendly ex's inner exception message and ex's message are not equal");
        }
    }
}
