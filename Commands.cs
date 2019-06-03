using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using System.IO;

// Commands with no prefix
public class Info : ModuleBase
{
    // Data ------------------------------------------------
    public static Riddle[] riddles = new Riddle[] { };
    public static Dictionary<ulong, Session> sessions = new Dictionary<ulong, Session>();
    public static string[] data = new string[] { };
    public static int currentRiddle;

    // Helper functions ------------------------------------

    // Resets variables after guessing is done
    public void Reset(ulong currentChannel)
    {
        sessions[currentChannel].ClueNo = 0;
        sessions[currentChannel].CurrentRiddle = -1;
        if (sessions[currentChannel].PreviousRiddles.Count == riddles.Length)
            sessions[currentChannel].PreviousRiddles.Clear();
    }

    // Initializes program data
    public static void Initialize()
    {
        char choice;
        string directory = null;
        bool modeChosen = false;

        // Choose bot language
        while (!modeChosen)
        {
            Console.WriteLine("Please choose bot language:");
            Console.WriteLine("(E)nglish");
            choice = Console.ReadKey().KeyChar;
            switch (choice)
            {
                case 'E':
                case 'e':
                    modeChosen = true;
                    directory += "/en";
                    break;
                default:
                    Console.Write(Environment.NewLine + "Invalid choice." + Environment.NewLine);
                    Console.Clear();
                    break;
            }
        }
        Console.Write(Environment.NewLine);

        if (riddles.Length == 0)
            riddles = JsonConvert.DeserializeObject<Riddle[]>(File.ReadAllText(Directory.GetCurrentDirectory() + directory + "/Riddles.json"));
        if (data.Length == 0)
            data = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Directory.GetCurrentDirectory() + directory + "/Data.json"));
    }

    // Commands --------------------------------------------

    // ~help
    [Command("help"), Summary("Bot prints its commands.")]
    [Alias("commands")]
    public async Task Help()
    {
        await ReplyAsync(data[19] + "\n\n" + data[20] + "\n" +
                         data[21] + "\n" + data[22] + "\n" +
                         data[23] + "\n" + data[24] + "\n" +
                         data[25]);
    }

    // ~guess 
    [Command("guess"), Summary("User guesses answer to riddle.")]
    [Alias("answer", "solution", "ans")]
    public async Task Guess([Remainder, Summary("User guess")] string guess)
    {
        // Variables
        int index = 0;
        string answer = null;

        // Characters string - characters that can appear before or after the answer
        string characters = " .!,?;'()[]";

        // Snarky line Jack will say if the guess was correct
        string sass = data[RandomNum.Rand(5, 11)];

        // If no riddle was given
        if (sessions[Context.Channel.Id] == null || sessions[Context.Channel.Id].CurrentRiddle < 0)
            await ReplyAsync(data[3]);

        // A riddle was given
        else
        {
            // Loops while there are answers available
            while (index < riddles[sessions[Context.Channel.Id].CurrentRiddle].Answers.Count)
            {
                answer = riddles[sessions[Context.Channel.Id].CurrentRiddle].Answers[index];

                // If guess is literally the answer
                if (guess.ToLower() == answer.ToLower() || guess.ToLower() == answer.ToLower() + "s")
                {
                    // Give sass
                    await ReplyAsync(sass);

                    // Add riddle to previously completed riddles
                    sessions[Context.Channel.Id].PreviousRiddles.Add(sessions[Context.Channel.Id].CurrentRiddle);

                    // Reset variables
                    Reset(Context.Channel.Id);

                    // Break loop
                    break;
                }

                // If the guess does contain the answer
                else if (guess.ToLower().Contains(answer.ToLower()))
                {
                    foreach (Char character in characters)
                    {
                        foreach (Char character2 in characters)
                        {
                            // Guess is deemed unobscured
                            if (guess.ToLower().Contains(character + answer.ToLower() + character2))
                            {
                                // Set index
                                index = riddles[sessions[Context.Channel.Id].CurrentRiddle].Answers.Count;

                                // Add riddle to previously completed riddles
                                sessions[Context.Channel.Id].PreviousRiddles.Add(sessions[Context.Channel.Id].CurrentRiddle);

                                // Give user sass
                                await ReplyAsync(sass);
                            }
                            if (sessions[Context.Channel.Id].PreviousRiddles.Contains(sessions[Context.Channel.Id].CurrentRiddle))
                                break;
                        }
                        if (sessions[Context.Channel.Id].PreviousRiddles.Contains(sessions[Context.Channel.Id].CurrentRiddle))
                        {
                            Reset(Context.Channel.Id);
                            break;
                        }
                    }
                    // Guess was found, but was deemed "obscured"
                    if (sessions[Context.Channel.Id].CurrentRiddle > 0)
                        await ReplyAsync(data[4]);
                }
                // If guess did not have this answer, go to next answer (if available)
                index++;
            }
        }

        // Guess is none of the answers
        try
        {
            if (!riddles[sessions[Context.Channel.Id].CurrentRiddle].Answers.Any(a => guess.ToLower().Contains(a)))
            {
                await ReplyAsync(data[11]);
            }
        }
        catch (ArgumentNullException) { }
        catch (IndexOutOfRangeException) { }
    }

    // ~give up
    [Command("give up"), Summary("User gives up on solving the riddle/question.")]
    public async Task GiveUp()
    {
        if (sessions[Context.Channel.Id] == null || sessions[Context.Channel.Id].CurrentRiddle < 0)
            await ReplyAsync(data[3]);
        else
        {
            Reset(Context.Channel.Id);
            await ReplyAsync(data[12]);
        }
    }

    // ~intro
    [Command("intro"), Summary("The bot introduces itself.")]
    [Alias("introduce yourself", "hello", "greetings", "salutations", "hi", "hey")]
    public async Task Introduction()
    {
        await ReplyAsync(data[2]);
    }

    // ~repeat riddle
    [Command("repeat riddle"), Summary("Repeats a given riddle.")]
    [Alias("repeat conundrum", "repeat enigma", "repeat puzzle", "repeat question")]
    public async Task RepeatRiddle()
    {
        // No log for current or previous channel sessions
        if (!sessions.ContainsKey(Context.Channel.Id))
            await ReplyAsync(data[3]);

        // Log exists and the previous riddle was solved
        else if (sessions[Context.Channel.Id].CurrentRiddle < 0)
            await ReplyAsync(data[3]);

        // Log and previous riddle was not solved
        else
            await ReplyAsync(riddles[sessions[Context.Channel.Id].CurrentRiddle].Main);
    }
}

// Give commands
[Group("give")]
[Alias("tell")]
public class Give : Info
{
    // ~give riddle
    [Command("riddle"), Summary("Gives a riddle.")]
    [Alias("conundrum", "enigma", "puzzle", "question")]
    public async Task Riddle()
    {
        // No log for current or previous channel sessions
        if (!sessions.ContainsKey(Context.Channel.Id))
        {
            // Add session to session list
            sessions.Add(Context.Channel.Id, new Session(RandomNum.Rand(0, riddles.Length), new List<int>(), 0));

            // Give riddle to user
            await ReplyAsync(riddles[sessions[Context.Channel.Id].CurrentRiddle].Main);
        }

        // Log exists and the previous riddle was solved
        else if (sessions[Context.Channel.Id].CurrentRiddle < 0)
        {
            // Get random riddle not yet played
            do
            {
                currentRiddle = RandomNum.Rand(0, riddles.Length);
            } while (sessions[Context.Channel.Id].PreviousRiddles.Contains(currentRiddle));

            // Save current riddle to session
            sessions[Context.Channel.Id].CurrentRiddle = currentRiddle;

            // Give riddle to user
            await ReplyAsync(riddles[currentRiddle].Main);
        }

        // No log and previous riddle was not solved
        else
            await ReplyAsync(data[13]);
    }

    // ~give new riddle
    [Command("new riddle"), Summary("Gives a new riddle.")]
    [Alias("new conundrum", "new enigma", "new puzzle", "new question")]
    public async Task NewRiddle()
    {
        // There is a current riddle session
        if (sessions.ContainsKey(Context.Channel.Id))
        {
            // Reset session variables
            Reset(Context.Channel.Id);

            // Riddles were previously solved
            if (sessions[Context.Channel.Id].PreviousRiddles.Count > 0)
            {
                do
                {
                    currentRiddle = RandomNum.Rand(0, riddles.Length);
                } while (sessions[Context.Channel.Id].PreviousRiddles.Contains(currentRiddle));
            }
            else
                currentRiddle = RandomNum.Rand(0, riddles.Length);

            // Save current riddle to session
            sessions[Context.Channel.Id].CurrentRiddle = currentRiddle;

            // Show riddle in chat
            await ReplyAsync(data[18]);
            await ReplyAsync(riddles[sessions[Context.Channel.Id].CurrentRiddle].Main);
        }
        else
        {
            currentRiddle = RandomNum.Rand(0, riddles.Length);
            sessions.Add(Context.Channel.Id, new Session(currentRiddle, new List<int>(), 0));
            await ReplyAsync(riddles[sessions[Context.Channel.Id].CurrentRiddle].Main);
        }
    }

    // ~give clue
    [Command("clue"), Summary("Returns a clue.")]
    [Alias("hint")]
    public async Task Clue()
    {
        // A riddle was given to the user
        if (sessions[Context.Channel.Id] != null && sessions[Context.Channel.Id].CurrentRiddle > 0)
        {
            // No clues available
            if (riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints.Count == 0)
                await ReplyAsync(data[14]);

            // Clues available; give one clue
            else if (sessions[Context.Channel.Id].ClueNo < riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints.Count)
            {
                await ReplyAsync(riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints[sessions[Context.Channel.Id].ClueNo]);
                sessions[Context.Channel.Id].ClueNo++;
            }

            // No more clues available
            else
                await ReplyAsync(data[15]);
        }

        // No riddle was given
        else
            await ReplyAsync(data[16]);
    }

    // ~give clues
    [Command("clues"), Summary("Returns all the clues at once.")]
    [Alias("hints")]
    [RequireOwner]
    public async Task Clues()
    {
        // A riddle was given to the user
        if (sessions[Context.Channel.Id] != null && sessions[Context.Channel.Id].CurrentRiddle > 0)
        {
            // No clues available
            if (riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints.Count == 0)
                await ReplyAsync(data[14]);

            // Clues available; give one clue
            else if (sessions[Context.Channel.Id].ClueNo < riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints.Count)
            {
                while (sessions[Context.Channel.Id].ClueNo < riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints.Count)
                {
                    await ReplyAsync(riddles[sessions[Context.Channel.Id].CurrentRiddle].Hints[sessions[Context.Channel.Id].ClueNo]);
                    sessions[Context.Channel.Id].ClueNo++;
                }
            }

            // No more clues available
            else
                await ReplyAsync(data[15]);
        }

        // No riddle was given
        else
            await ReplyAsync(data[16]);
    }

    // ~give answer
    [Command("answer"), Summary("Returns the answer to the riddle.")]
    [Alias("ans", "solution")]
    [RequireOwner]
    public async Task Answer()
    {
        if (sessions[Context.Channel.Id] == null || sessions[Context.Channel.Id].CurrentRiddle < 0)
            await ReplyAsync(data[3]);
        else
        {
            await ReplyAsync(data[17] + riddles[sessions[Context.Channel.Id].CurrentRiddle].Answers[0] + ".");
            Reset(Context.Channel.Id);
        }
    }
}
