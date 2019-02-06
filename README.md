# MilkshakeCupBot
Telegram bot for the prestigious milkshake cup

Just a simple telegram bot to keep track of a FIFA tournament.
There are two Groups: GroupA and GroupB. each group is stored in a csv file.

Commands:
=========
you can execute the following commands:

/tablas {group} : returns the current standing for the group. <group> is optional. if not present or different than "a" or "b"
                  then it will return the standing for both groups. <group> is case insensitive.

/marcador {teamOrPlayer1} {goalsScoredByTeamOrPlayer1} {teamOrPlayer2} {goalsScoredByTeamOrPlayer2} : Updates the standing of
                  standing of a group with the score of the match played. <teamOrPlayer> are case insensitive. Assumes that
                  both teamOrPlayer 1 and 2 are from the same group.
                  
How To run
==========
After copying the git repository, just run: dotnet run
