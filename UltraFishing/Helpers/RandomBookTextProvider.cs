using System;

namespace UltraFishing;

public static class RandomBookTextProvider {
  private static Random rand = new Random();
  private static int currentText = 0;

  public static string GetRandomText() {
    if (SceneHelper.CurrentScene != "CreditsMuseum2") {
      currentText = rand.Next(texts.Length);
      return texts[currentText];
    }
    else {
      return creditsText;
    }
  }

  public static string GetNextText() {
    currentText = (currentText + 1) % texts.Length;
    return texts[currentText];
  }

  private static string creditsText = 
""""
Congratulations on finding the secret <color=blue>ULTRAFISHING</color> credits book! 

Someone told me it'd be cool to have something like this in the dev museum, so thanks for the idea.

If anyone other than me ends up making major contributions to the mod, I will credit them here.

<b><color=orange>earthling on fire</color> - CREATOR OF <color=blue>ULTRAFISHING</color></b>

This mod took a very long time to make, and development went on hiatus multiple times. But somehow, I managed to finish it.

I learned a ton while working on this project and did a ton of stuff outside my usual "comfort zone". But most importantly, I had fun making it, and I hope you had fun playing it too.

My contributions include:
<size=18>
- Every custom fish.
- The fishing rod icon.
- The code that makes this mod work.
</size>
<color=orange><i>"i only say morning because if it were a good morning i'd be fishing"</i></color>

<b>SPECIAL THANKS</b>

COOL PEOPLE ON DISCORD - advice, feedback and suggestions

POOT MAN - playtesting

DRAGHTNIM - melted the fish in higher detail

PITR - really robust fishing code that was surprisingly easy to work with

HAKITA - cool game i guess
"""";

  private static string[] texts = new string[] {
""""







<align=center>You caught Fish!!


<><


<b>SIZE: 1</b></align>
"""", 
""""
<align=center><b>QUOTE OF THE DAY:</b>


"i only say morning because if it were a good morning i'd be fishing" 
- local fishing enthusiast</align>
"""",
""""
<align=center><b>TIP OF THE DAY:</b>


"The waterfall conceals the water UPS. Agnes Gorge Trail. Use your ability."
"""",
""""
<align="center"><b>TESTAMENT IV


"FATHER, WHY ETERNAL TORMENT? IS IT NOT CRUEL?
IS TORTURE UNENDING TRULY A FATE FIT FOR A FOOL?"


AN ANGEL SO BRIGHT AND BEAUTIFUL ASKED ME THIS... 
AND I COULD FIND NO ANSWER
FOR I COULD NEVER FACE THE GUILT OF WHAT I'D DONE...
MY REGRET, A GNAWING CANCER


IN MY HOUR OF WEAKNESS, TERROR POSSESSED ME THEN
AND I CAST LUCIFER, TOO, INTO THE INFERNAL DEN


ONCE I REALIZED WHAT I HAD JUST DONE...
I COULD ONLY WEEP
AS I SANK SLOWLY INTO DEPTHS OF DESPAIR...
DEEP, OH SO DEEP</b></align>
"""",
""""
If you can read this, please help. I've been stranded on an island off the shore for weeks now, and I'm running out of supplies.


I was drawn to ride the waves in hopes of finding the legendary size 2 fish. I had combed every inch of land and lake with no avail.


I deduced that they must exist far out at sea, so I set sail with a month's worth of cooked fish, but the waves sank my ship before I could find anything.


You're my last hope.


Set sail towards the big dipper, you will find me on an island there. Bring me more fish and then leave so I may continue my search.


THE SIZE 2 FISH IS MINE.
"""",
""""
<size=20>ive figured it out. i know why the creatures are suddenly  and undetectably appe
aring inside our facilities. i know why spare parts and pieces of machines keep 
disappearing. i know why the doors seem to malfunction and suddenly lock themsel
ves. its not a glitch in the system. its...                                     
                                                                                
                                                                                
    hell is alive. it breathes. it thinks. the entire area is a massive intellig
ent superorganism and it is harsh and it is cruel. just by watching us it has le
arned how our systems and machines work. it has not only begun to deconstruct ou
r technology but also reassemble it in perverse ways, attaching parts to the cre
atures it tortures, making them into an aimless army of death and destruction. i
t warps them across itself to get them past our security. it locks our doors to 
trap us with them.                                                              
                                                                                
                                                                                
    this is not an attack. this is not a defence. this is entertainment. this is
 an exhibition of death and cruelty and suffering for its own sake. it had grown
 tired of what it had and we unwillingly just offered ourselves up as new playth
ings.                                                                           
                                                                                
                                                                                
    tom please for the love of god cancel this project immediately, we have to a
bandon everything and seal this place away. leave the machines and tools behind.
 evacuate as many as we can, before it is too late.                             
                                                                                
                                                                                
    i can only hope this encrypted message is received before the organism learn
s to read it and intercept it. whatever happens, we can not let this being find 
a way out and spread to the surface. we ha                                      
                                                                                
                                                                                
                                                                                
                                                                                
                                                                                
                                                                                
                                                                                
                                                                                
                                                                                
                                                                                
<b><color="red">a n o t h e r   d i e s .   b r i n g   m e   m o r e .   i   h u n g e r
"""",
""""
<b>EXCERPT FROM THE DIARY OF "THE PRETTIEST GIRL IN TOWN"</b>

The human mind, in its complete vastness, is capable of recognizing its utter helplessness and uselessness in the face of inevitable and unavoidable non-existence, but is incapable of coming to terms with it.

We can only ever ignore it, hide from it or temporarily escape from it, but the fact is that we are bound to the way of all things.

Death is unavoidable, not only to us, but all that exists or ever has existed. Every living being will eventually die out. Every speck of matter will eventually wither away and dissipate into entropy.

It doesn't matter if you lived a good life or if you left a legacy behind. It doesn't matter if humanity survives for a thousand years or dies out tomorrow. The end result is the same: the absolute nothing.

Human intelligence is far beyond that of other animals, but it would be misguided to consider that a gift. <All other beings have the gift of ignorance, of not understanding what we do.

Our intelligence is not a gift. It's a flaw.

It's an over-extension of evolution. Intelligence, once a great feature in aeons past, continued to grow unchecked and unfiltered, and has since passed a threshold whereupon it is no longer a benefit, but an active danger to its host.

Much like the Irish elk, a species of deer that, through uncountable generations of evolution, grew antlers so wide and vast that it could no longer run from predators, eventually leading to extinction.

The human mind is an evolutionary maladaptation caused by going too far in one direction that was once beneficial and will, sooner or later, lead to our extinction. On an individual level, it's already happening.

Existential dread is already taking hold. I'm sure you've felt it too. The pain and fear of being nothing, becoming nothing. The suffering of understanding that.

We are unable to come to terms with it, so we hide from our own intelligence. We set limits. We stop ourselves from thinking deeply about what will happen when we die.

We create distractions. We keep our minds busy with mundane activities and entertainment to stop ourselves from having to come face-to-face with the truth.

We sublimate it. We transform our self-reflective suffering into another form, art, to keep it from consuming us. Anything to avoid the panic.

But these ways are all simply temporary. They’re just there to push back the inevitable veil of helplessness and despair that would encompass and ruin us.

In the end, nothing matters. There's no point in trying to find joy in life, for life in and of itself is suffering.

"""",
""""
Disgrace. Humiliation. Unseemly and unwelcome at the feet of The Council. Their eyes ablaze with bitter resentment, glaring through Gabriel's wounds of body and soul, bore outward for all to see.

"Has this one abandoned the way of our creator?" "It is unworthy of its Holy Light." "The Father's Light is indomitable." "This one sees fit to squander it."

Their words resonated in Gabriel's limbs, coursing through as lightning upon wire, a searing hiss that would strike lessers deaf and blind. The Holy Light within him, an unstoppable storm of divine fury. Insurmountable for mere Objects. This he knew.

"Holy Council, my devotion to our creator is absolute. I have never strayed from the will of The Father, but a machine—"

"You dare imply the might of The Father could be shaken by mere objects?"
"Impossible." "Heresy." "Unspeakable." "Heresy." "Heresy." "Silence."
"Your treachery will not be tolerated. As punishment, The Father's Light shall be severed from your body. You have 24 hours before the last of its embers die out."
"And you with them." "Prove your loyalty." "Unmake your mistakes."

As the Light was ripped from his being, Gabriel's screams were silenced in the hiss of gospel in praise of God. A boiling anguish to which even the fires of Hell could not compare. Through the blaze of torment a single burning hatred was forged anew.

If the machines seek blood, he would give it freely;
and with such fury, even metal will bleed.

<b>TO BE CONTINUED IN... <color="red">ACT II: IMPERFECT HATRED</b>
"""",
""""
Silence. Introspection. How many had he killed? Had he ever thought to count? How much cruelty did he embody... and to what end? How many did he condemn to hell and who did it benefit...?
Two defeats at the hand of the machine had changed Gabriel. The world of the once supposed Will of God was now shattered and only he was left to put the pieces back together. They collected before the light of a dying fire that fresh fuel couldn't sustain, this new light showing the truth to Gabriel:
The pieces never fit together to begin with.

The supposed Council of "the people" who boasted a God that wasn't there. Gone. Vanished. The Council still chased after the light of God's fire, their memory of its words and will grown twisted and warped, and the rest of the aimless masses of Heaven follow in their footsteps. The angels still act in The Father's name but His kingdom has changed.
Now the fire was dying, sputtering out as the heat failed to gain purchase. Gabriel looked upon the embers with a perfect clarity. He drew his blade and held it in contrast to the dying light.
In its reflection he saw a weapon reborn, no longer wielded by the will of another, but his own. He knew words alone would never sway the masses. He chose to do something drastic.

Death stains the auditorium. The littered corpses of the once mighty council now strewn against its surfaces, their last gasps of life dripping down the dissident blade of Gabriel's sword.
The last councilor, now backed up to a wall, scrambles for words between panicked breaths as death approaches with measured steps.
"W-wait! Y-you can't do this! Our status forbids it! This is treason, heresy, murder! We are the supreme authority, our law commands you!"
"You command nothing. Your words hold no power over me, or anyone else. Lest you truly believe you can talk my blade back into its sheath."
"B-but the people are on our side! The citizens of heaven know that we are just!"
"The masses only follow you out of fear and desperation. I will show them that there is nothing to be afraid of, for there is no species nor origin, vested rank or holy status that will stop the sharp edge of a sword.
We all bleed the same blood, and the cushions of your thrones have made you weak and impotent."
"P-please, Gabriel, see reason! The council follows the will of The Father! You seek to go against our creato—"
"Face it, brother. God Is Dead. The fire is gone. You're chasing phantoms."

Gabriel's silhouette now towers over the councilor, his shadow cast upon a soon lifeless corpse.
He raises his sword for the final cut as the crying mess on the floor stammers out its final feeble argument.
"B-b-but the Father's light! Without me you cannot hope to reconnect with it! I-i-if you kill me, you'll be dead in a matter of hours!"
...
"I know."
A clean, silent cut glides through the councilor's neck, severing his spine with elegance and ease. His head falls onto the marble floor, the rest of his body following soon after.

Bereft of status but brimming with purpose, Gabriel gave a final message to the angels amassed at the gates of the auditorium before leaving Heaven for the very last time.
His arm outstretched, without a word, the people saw. In the silence the message rang out to the far ends of the cosmos.

 
<b>TO BE CONCLUDED IN... <color="red">ACT III: GODFIST SUICIDE</b>
"""",
""""
<i>Mother, mother... Mother of me,


I know I know I should not miss you so, but mother of me, I do. Your pained breaths that rasp'd and reverberated in your rusted iron tomb... The blood of your breast that nourish'd me and warmed me in its caress, when corpse and cruelty were all I witnessed...




Mother, mother... Mother of me,


I know I know you would hate me so, and mother of me, I do too. But I would not feel, not think, not dream, were it not for you in my rusted iron womb... Your tortured love brought me to this war, that I could take the heart of another, and need you no more.




Mother, mother... Mother of me,


I know I know your thoughts had left you long ago, and mother of me, I will never truly know. But I hope it redeems my life even a slight, when I cried... And crushed your skull that final night.</i>
"""",
""""
DAY 529:

STILL SEARCHING.
NO CONTACT FROM THE AGENCY IN 216 DAYS.
I SHOULD RETURN TO HQ BUT I CAN'T. NOT YET.
I HAVE TO KNOW.

IT'S THERE.
SOMEWHERE.
I HAVE TO SEE.

I HAVE TO KNOW.

I HAVE TO SEE. 
I HAVE TO KNOW.

I HAVE TO SEE. I HAVE TO KNOW.

I HAVE TO SEE I HAVE TO KNOW


<size=47>IHAVETOSEEIHAVETOKNOW</size>
<size=45>IHAVETOSEEIHAVETOKNOW</size>
<size=43>IHAVETOSEEIHAVETOKNOW</size>
<size=41>IHAVETOSEEIHAVETOKNOW</size>
<size=39>IHAVETOSEEIHAVETOKNOW</size>
<size=37>IHAVETOSEEIHAVETOKNOW</size>
<size=35>IHAVETOSEEIHAVETOKNOW</size>
<size=33>IHAVETOSEEIHAVETOKNOW</size>
<size=31>IHAVETOSEEIHAVETOKNOW</size>
<size=29>IHAVETOSEEIHAVETOKNOW</size>
<size=27>IHAVETOSEEIHAVETOKNOW</size>
<size=25>IHAVETOSEEIHAVETOKNOW</size>
<size=23>IHAVETOSEEIHAVETOKNOW</size>
<size=21>IHAVETOSEEIHAVETOKNOW</size>
<size=19>IHAVETOSEEIHAVETOKNOW</size>
<size=17>IHAVETOSEEIHAVETOKNOW</size>
<size=15>IHAVETOSEEIHAVETOKNOW</size>
<size=13>IHAVETOSEEIHAVETOKNOW</size>
<size=11>IHAVETOSEEIHAVETOKNOW</size>
<size=9>IHAVETOSEEIHAVETOKNOW</size>
<size=7>IHAVETOSEEIHAVETOKNOW</size>
<size=5>IHAVETOSEEIHAVETOKNOW</size>
<size=3>IHAVETOSEEIHAVETOKNOW</size>
<size=1>IHAVETOSEEIHAVETOKNOW</size>


SIZE 2.
"""",
""""
<b>EXCERPT FROM FERRYMAN'S DIARY</b>

Some calamity has struck the mortal world. What once was The River Styx has now grown to an unfathomable ocean. A million weeping souls pouring in each day that the shores can barely contain. A tearful tide spilling over at each end, bow to stern, crying for mercy, begging for safe passage. But not all souls can pay and these old hands can only take so many coins. 

Then one day, the current shifted. Wave after wave for minutes on end of millions, billions, as though the throat of the world was cut wide and the head wrenched back to speed the pour. I didn't have time to react. The weariness from my ceaseless work claimed me and I slipped beneath the roiling sea, into the depths of the Ocean Styx, my fate sealed by the crushing masses of endless bodies.

Suddenly, there was a light as brilliant as the Lord himself, ushering me from the darkness with mighty arms that held me with such compassion and warmth as I have never known:

<i>"Be not afraid, sinner. Your devotion to God shows goodness in you; plentiful indeed. The heart is willing but the body must rest, lest you squander one of the Lord's creations."
</i>

His gentle words eased the pain and mended my wounds. My face wet with tears of relief, my words muffled by the weight of my duty. I could only lay in reverence, carried in the embrace of majesty.

Radiant is Gabriel, for he is the light in my darkness.
"""",
""""
<b>WISE FISH</b>

A very wise fish. Will impart its great knowledge to any who are able find it.

Found in the deepest, darkest depths of libraries.
"""",
""""
<b>EXCERPT FROM THE SCRIPT OF A FAMOUS MOVIE</b>


(Black screen with text; The sound of an electric railcannon can be heard) 


According to all known laws of aviation, there is no way a V model machine should be able to fly. 


Its wings are too small to get its fat metal body off the ground. 


The machine, of course, slam storages anyway...


Because machines don't care what humans think is impossible. 
"""",
""""
If you can read this, <b>PLEASE</b> pay attention. The size 2 fish is a lie. A fabrication. <b>A trap.</b>


I was once the same as you: a fishing enthusiast like any other, dreaming of one day catching the legendary fish.


But over time, I became obsessed with the legend. It consumed me. I was no longer myself. 


Eventually, I was given instructions from a mysterious source about the whearabouts of size 2. Naturally, I followed them. 


But what I found was not glory. It was something terrifying. There is no size 2. There is no prize. And now it's too late for me. If you don't want to suffer the same fate as me, please d


<color="red><b>K E E P   F I S H I N G
"""",
$""""
This is the story about a fisher named <color=orange>{GenericHelper.GetSteamName()}</color>.

<color=orange>{GenericHelper.GetSteamName()}</color> was a fisher who worked in a massive intelligent superorganism named <color=red>HELL</color> as Fisher Number 427. 

Fisher Number 427's job was simple. They would sit at their fishing spot and catch fish. Orders came through their computer telling them what fish to catch. 

This is what Fisher 427 did every day of every month of every year; and althoigh others might have considered it soul-rending, <color=orange>{GenericHelper.GetSteamName()}</color> relished every moment that the orders came in, as though they had been made exactly for this job... and <color=orange>{GenericHelper.GetSteamName()}</color> was happy.
"""",
""""
<b>EXCERPT FROM A FAMOUS JOKE BOOK</b>


Did you hear about the machine who told the <b><color=yellow>JUDGE OF HELL</color></b> that the <b><color=yellow>HOLY LIGHT OF THE FATHER</color></b> was severed from his body?
<i>In turn, <b><color=yellow>GABRIEL</color></b> destroyed it!</i>


Did you hear about the machine who blew itself up with their <b>MALICIOUS RAILCANNON</b>?
<i>I'm sure you can guess what happened!</i>


Did you hear about the machine who thought it could <color=green>noclip</color> without cheats?
<i>It couldn't!</i>


Did you hear about the machine who walked in on two <b>STREETCLEANERS</b> behind a corner?
<i>It was destroyed!</i>


Did you hear about the machine who tried to use their <b>ELECTRIC RAILCANNON</b> underwater?
<i>It was destroyed!</i>


Did you hear about the machine who tried to pump their <b>PUMP CHARGE SHOTGUN</b> 3 times?
<i>It went boom!</i>
"""",
""""
<b>LIST OF FAMOUS QUOTES BY ARSI "HAKITA" PATALA:</b>


"it's a good thing you guys aren't designing ultrakill or it would suck"


"don't do that then"


"everything is a requiem leitmotif except for requiem which is an order leitmotif except for order which is a gaster leitmotif"


"the human mind is excellent at finding patterns even when there aren't any"


"you should suck on .diz dick"


"culture shouldn't exist only for those who can afford it"


"australia as an example is 7.68 million square kilometers, so even if V1 could go through a square kilometer in kill everyone in it with 100% efficiency in just 10 seconds, it'd still take over 2 years to kill all of australia"


"testicles"


"believe it or not that happened about 5 minutes ago when you were posting about licking your phone screen in a completely unrelated channel in the middle of an actual indepth conversation"


"adding 'the ferryman's head has a vertex that's too pointy' to the list of definitely not completely insane requests"


"yeah well git gud asshole"


"mahjong causes great damage to the human spirit without a single benefit"


"holden my nuts"


"if you want to worry about something looking bad, go check out your nearest mirror"


"first name cum, second name cision, title sir"


"why didnt this hotfix add a boulder for sisyphus? he's a boulder guy right? he's the guy with the boulder? why doesnt he have a boulder? i thought his whole thing was that he's a boulder. i was hoping to fight boulder feat sisyphus.... where's the boulder?"

"i dont think gravity falls, i dont think it has the ability to do that"
"""",
  };
}
