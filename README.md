# RiddleBot
**RiddleBot** is a *Discord* bot that gives riddles, made using the [Discord.Net](https://github.com/discord-net/Discord.Net) library. 
The base bot code was made by following the guides available at the [Discord Net Documentation Website](https://discord.foxbot.me/stable/guides/introduction/intro.html).
Originally conceptualised for use in the [*MediEvil* Discord server](https://discordapp.com/invite/X84wKBp) as [Jack of the Green](https://gallowmere.fandom.com/wiki/Jack_of_the_Green), a character who gives out riddles in the *MediEvil* game.

## How to use

1. You will need to create a new application on the [Discord Applications portal](https://discordapp.com/developers/applications/). 
You can follow the walkthrough available [here](https://discord.foxbot.me/stable/guides/getting_started/first-bot.html).
2. Then, you can download [one of the releases available here on GitHub](https://github.com/RampantSpirit/RiddleBot/releases) 
or download the source to compile the bot yourself.
3. When initialising the bot, you will first be prompted to select a language and then you will be prompted for your bot token, which can be found in the Discord Applications portal under the Bot tab for your application.
4. Congratulations, you should now have a running RiddleBot. If you want it running 24/7, considering [various deployment options available](https://discord.foxbot.me/stable/guides/deployment/deployment.html) or buying a Raspberry Pi for deployment.
5. Enjoy!

## Commands

The bot has the following commands available:

### General

- !intro - *The bot will introduce itself.*
- !help - *Prints some of the commands.*
- !give riddle - *The bot will say a riddle, unless a riddle was already said previously.*
- !give new riddle - *Gives a new riddle, even if a riddle was previously said.*
- !repeat riddle - *Repeats the already given riddle.*
- !give clue - *Gives a clue to the riddle (if available).*
- !guess - *Follow this command up with your answer to the riddle to make a guess.*
- !give up - *Ends current riddle.*

### Owner only

- !give clues - *Lists all the available clues at once.*
- !give answer - *Answers the riddle.*

## What to contribute

All contributions are welcome! The bot especially needs the following:

1. More riddles.
2. Clues for riddles that have no clues.
3. Translations into other languages.

All of these can be accomplished by editing and/or copying the **Data.json** and **Riddles.json** files.
