using System.Collections.ObjectModel;


namespace BlackJackForOne.Application;

public sealed record HandState(
    ReadOnlyCollection<string> Cards,
    int Value,
    int IsActiveID,
    bool Busted = false
);

public sealed record Result(
    Replies Reply,
    Replies BlackjackInfo,
    List<HandState> HandStates
); 

public sealed record DealerResult(
    ReadOnlyCollection<string> Cards,
    Replies Reply,
    int Value
);
public sealed record WinnerResult(
    HandState Player,
    DealerResult Dealer,
    Replies WinCondition
);