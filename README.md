# MilkshakeCupBot
Telegram bot for the prestigious milkshake cup

Just a simple telegram bot to keep track of a FIFA tournament.
There are three Groups: a, b and c. each group is stored in a csv file.

Commands:
=========
you can execute the following commands:

/tablas : returns the current standing for each group.

/tabla {group} : returns the current standing for the specific group.

/marcador {teamOrPlayer1} {goalsScoredByTeamOrPlayer1} {teamOrPlayer2} {goalsScoredByTeamOrPlayer2} : Updates the standing of
                  standing of a group with the score of the match played. <teamOrPlayer> are case insensitive. Assumes that
                  both teamOrPlayer 1 and 2 are from the same group.
 
 /borrar {teamOrPlayer1} {goalsScoredByTeamOrPlayer1} {teamOrPlayer2} {goalsScoredByTeamOrPlayer2} : Erases a result from the group
                 
How To run
==========
After copying the git repository, just run: dotnet run
