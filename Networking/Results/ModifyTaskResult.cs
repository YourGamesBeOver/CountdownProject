namespace Countdown.Networking.Results {
    public enum ModifyTaskResult {
        Success,    //server returned status=true
        Failure,    //server returned status=false
        Error       //server did not return a 200
    }
}
