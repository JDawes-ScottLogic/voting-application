DECLARE @voteCount INT;
SET @voteCount  = 0;

/* EDIT Below */
DECLARE @voteTotal INT;
SET @voteTotal  = 1000;

DECLARE @pollId INT;
SET @pollId = 1;

DECLARE @optionCount INT;
SET @optionCount = 1;
/* EDIT Above */

While @voteCount < @voteTotal
BEGIN
	DECLARE @newGuid uniqueidentifier
	SET @newGuid = NEWID()
	
	INSERT INTO Ballots (TokenGuid) VALUES (@newGuid);
	INSERT INTO Votes (Ballot_Id, Choice_Id, Poll_Id, VoteValue) VALUES (@@IDENTITY, (SELECT Id FROM Choices WHERE PollChoiceNumber =(FLOOR(RAND()*@optionCount)+1) AND Poll_Id = @pollId), @pollId, 1);
	
	SET @voteCount = @voteCount + 1;
END;