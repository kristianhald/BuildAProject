using System;

namespace BuildAProject.BuildManagement.NUnit.Runners
{
  public class NUnitTestMethodResult : IEquatable<NUnitTestMethodResult>
  {
    public NUnitTestMethodResult(string methodName, NUnitStatus status, string message)
    {
      if (methodName == null)
      {
        throw new ArgumentNullException("methodName");
      }

      if (message == null)
      {
        throw new ArgumentNullException("message");
      }

      MethodName = methodName;
      Status = status;
      Message = message;
    }

    public string MethodName { get; private set; }

    public NUnitStatus Status { get; private set; }

    public string Message { get; private set; }

    public bool Equals(NUnitTestMethodResult other)
    {
      return
        other != null &&
        String.Equals(MethodName, other.MethodName) &&
        Status == other.Status &&
        String.Equals(Message, other.Message);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as NUnitTestMethodResult;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return MethodName.GetHashCode();
    }

    public override string ToString()
    {
      var stateDescription = String.Format("{0}: {1}", MethodName, Status);

      if (!String.IsNullOrWhiteSpace(Message))
      {
        stateDescription += String.Format(" - {0}", Message);
      }

      return stateDescription;
    }
  }
}
