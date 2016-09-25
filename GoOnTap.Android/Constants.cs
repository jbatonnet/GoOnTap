using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

public class PokemonInfo
{
    public int Id { get; }

    public string EnglishName { get; }
    public string FrenchName { get; }
    public string GermanName { get; }

    public int HP { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int SpecialAttack { get; }
    public int SpecialDefense { get; }
    public int Speed { get; }

    public int BaseStamina { get; }
    public int BaseAttack { get; }
    public int BaseDefense { get; }

    public PokemonInfo(int id, string englishName, string frenchName, string germanName, int baseAttack, int baseDefense, int baseStamina)
    {
        Id = id;

        EnglishName = englishName;
        FrenchName = frenchName;
        GermanName = germanName;

        BaseAttack = baseAttack;
        BaseDefense = baseDefense;
        BaseStamina = baseStamina;
    }

    public int GetCP(double level, int attack, int defense, int stamina) => Max(10, (int)(((BaseAttack + attack) * Sqrt(BaseDefense + defense) * Sqrt(BaseStamina + stamina) * Pow(Constants.CPMultipliers[level], 2)) / 10));
    public int GetMinimumCP(double level) => GetCP(level, 0, 0, 0);
    public int GetMaximumCP(double level) => GetCP(level, 15, 15, 15);

    public int GetHP(double level, int stamina) => Max(10, (int)((BaseStamina + stamina) * Constants.CPMultipliers[level]));
    public int GetMinimumHP(double level) => GetHP(level, 0);
    public int GetMaximumHP(double level) => GetHP(level, 15);

    public string GetLocalizedName(string locale = null)
    {
        string name = EnglishName;

        if (locale.StartsWith("fr") && FrenchName != null)
            name = FrenchName;
        if (locale.StartsWith("de") && GermanName != null)
            name = GermanName;

        return name;
    }
    public static double GetLevelAngle(double level, int playerLevel) => (Constants.CPMultipliers[level] - 0.094) * 202.037116 / Constants.CPMultipliers[playerLevel];
}

public partial class Constants
{
    public static Dictionary<double, double> CPMultipliers { get; } = new Dictionary<double, double>()
    {
        [1.0] = 0.094,
        [1.5] = 0.135137432,
        [2.0] = 0.16639787,
        [2.5] = 0.192650919,
        [3.0] = 0.21573247,
        [3.5] = 0.236572661,
        [4.0] = 0.25572005,
        [4.5] = 0.273530381,
        [5.0] = 0.29024988,
        [5.5] = 0.306057377,
        [6.0] = 0.3210876,
        [6.5] = 0.335445036,
        [7.0] = 0.34921268,
        [7.5] = 0.362457751,
        [8.0] = 0.37523559,
        [8.5] = 0.387592406,
        [9.0] = 0.39956728,
        [9.5] = 0.411193551,
        [10.0] = 0.42250001,
        [10.5] = 0.432926419,
        [11.0] = 0.44310755,
        [11.5] = 0.4530599578,
        [12.0] = 0.46279839,
        [12.5] = 0.472336083,
        [13.0] = 0.48168495,
        [13.5] = 0.4908558,
        [14.0] = 0.49985844,
        [14.5] = 0.508701765,
        [15.0] = 0.51739395,
        [15.5] = 0.525942511,
        [16.0] = 0.53435433,
        [16.5] = 0.542635767,
        [17.0] = 0.55079269,
        [17.5] = 0.558830576,
        [18.0] = 0.56675452,
        [18.5] = 0.574569153,
        [19.0] = 0.58227891,
        [19.5] = 0.589887917,
        [20.0] = 0.59740001,
        [20.5] = 0.604818814,
        [21.0] = 0.61215729,
        [21.5] = 0.619399365,
        [22.0] = 0.62656713,
        [22.5] = 0.633644533,
        [23.0] = 0.64065295,
        [23.5] = 0.647576426,
        [24.0] = 0.65443563,
        [24.5] = 0.661214806,
        [25.0] = 0.667934,
        [25.5] = 0.674577537,
        [26.0] = 0.68116492,
        [26.5] = 0.687680648,
        [27.0] = 0.69414365,
        [27.5] = 0.700538673,
        [28.0] = 0.70688421,
        [28.5] = 0.713164996,
        [29.0] = 0.71939909,
        [29.5] = 0.725571552,
        [30.0] = 0.7317,
        [30.5] = 0.734741009,
        [31.0] = 0.73776948,
        [31.5] = 0.740785574,
        [32.0] = 0.74378943,
        [32.5] = 0.746781211,
        [33.0] = 0.74976104,
        [33.5] = 0.752729087,
        [34.0] = 0.75568551,
        [34.5] = 0.758630378,
        [35.0] = 0.76156384,
        [35.5] = 0.764486065,
        [36.0] = 0.76739717,
        [36.5] = 0.770297266,
        [37.0] = 0.7731865,
        [37.5] = 0.776064962,
        [38.0] = 0.77893275,
        [38.5] = 0.781790055,
        [39.0] = 0.78463697,
        [39.5] = 0.787473578,
        [40.0] = 0.79030001,
        [40.5] = 0.7931164,
    };

    public static PokemonInfo[] Pokemons { get; } = new PokemonInfo[]
    {
        new PokemonInfo(1,   "Bulbasaur",  "Bulbizarre", "Bisasam",    126, 126, 90),
        new PokemonInfo(2,   "Ivysaur",    "Herbizarre", "Bisaknosp",  156, 158, 120),
        new PokemonInfo(3,   "Venusaur",   "Florizarre", "Bisaflor",   198, 200, 160),
        new PokemonInfo(4,   "Charmander", "Salamèche",  "Glumanda",   128, 108, 78),
        new PokemonInfo(5,   "Charmeleon", "Reptincel",  "Glutexo",    160, 140, 116),
        new PokemonInfo(6,   "Charizard",  "Dracaufeu",  "Glurak",     212, 182, 156),
        new PokemonInfo(7,   "Squirtle",   "Carapuce",   "Schiggy",    112, 142, 88),
        new PokemonInfo(8,   "Wartortle",  "Carabaffe",  "Schillok",   144, 176, 118),
        new PokemonInfo(9,   "Blastoise",  "Tortank",    "Turtok",     186, 222, 158),
        new PokemonInfo(10,  "Caterpie",   "Chenipan",   "Raupy",      62, 66, 90),
        new PokemonInfo(11,  "Metapod",    "Chrysacier", "Safcon",     56, 86, 100),
        new PokemonInfo(12,  "Butterfree", "Papilusion", "Smettbo",    144, 144, 120),
        new PokemonInfo(13,  "Weedle",     "Aspicot",    "Hornliu",    68, 64, 80),
        new PokemonInfo(14,  "Kakuna",     "Conconfort", "Kokuna",     62, 82, 90),
        new PokemonInfo(15,  "Beedrill",   "Dardargnan", "Bibor",      144, 130, 130),
        new PokemonInfo(16,  "Pidgey",     "Roucool",    "Taubsi",     94, 90, 80),
        new PokemonInfo(17,  "Pidgeotto",  "Roucoups",   "Tauboga",    126, 122, 126),
        new PokemonInfo(18,  "Pidgeot",    "Roucarnage", "Tauboss",    170, 166, 166),
        new PokemonInfo(19,  "Rattata",    "Rattata",    "Rattfratz",  92, 86, 60),
        new PokemonInfo(20,  "Raticate",   "Rattatac",   "Rattikarl",  146, 150, 110),
        new PokemonInfo(21,  "Spearow",    "Piafabec",   "Habitak",    102, 78, 80),
        new PokemonInfo(22,  "Fearow",     "Rapasdepic", "Ibitak",     168, 146, 130),
        new PokemonInfo(23,  "Ekans",      "Abo",        "Rettan",     112, 112, 70),
        new PokemonInfo(24,  "Arbok",      "Arbok",      "Arbok",      166, 166, 120),
        new PokemonInfo(25,  "Pikachu",    "Pikachu",    "Pikachu",    124, 108, 70),
        new PokemonInfo(26,  "Raichu",     "Raichu",     "Raichu",     200, 154, 120),
        new PokemonInfo(27,  "Sandshrew",  "Sabelette",  "Sandan",     90, 114, 100),
        new PokemonInfo(28,  "Sandslash",  "Sablaireau", "Sandamer",   150, 172, 150),
        new PokemonInfo(29,  "Nidoran♀",   "Nidoran♀",   "Nidoran♀",   100, 104, 110),
        new PokemonInfo(30,  "Nidorina",   "Nidorina",   "Nidorina",   132, 136, 140),
        new PokemonInfo(31,  "Nidoqueen",  "Nidoqueen",  "Nidoqueen",  184, 190, 180),
        new PokemonInfo(32,  "Nidoran♂",   "Nidoran♂",   "Nidoran♂",   110, 94, 92),
        new PokemonInfo(33,  "Nidorino",   "Nidorino",   "Nidorino",   142, 128, 122),
        new PokemonInfo(34,  "Nidoking",   "Nidoking",   "Nidoking",   204, 170, 162),
        new PokemonInfo(35,  "Clefairy",   "Mélofée",    "Piepi",      116, 124, 140),
        new PokemonInfo(36,  "Clefable",   "Mélodelfe",  "Pixi",       178, 178, 190),
        new PokemonInfo(37,  "Vulpix",     "Goupix",     "Vulpix",     106, 118, 76),
        new PokemonInfo(38,  "Ninetales",  "Feunard",    "Vulnona",    176, 194, 146),
        new PokemonInfo(39,  "Jigglypuff", "Rondoudou",  "Pummeluff",  98, 54, 230),
        new PokemonInfo(40,  "Wigglytuff", "Grodoudou",  "Knuddeluff", 168, 108, 280),
        new PokemonInfo(41,  "Zubat",      "Nosferapti", "Zubat",      88, 90, 80),
        new PokemonInfo(42,  "Golbat",     "Nosferalto", "Golbat",     164, 164, 150),
        new PokemonInfo(43,  "Oddish",     "Mystherbe",  "Myrapla",    134, 130, 90),
        new PokemonInfo(44,  "Gloom",      "Ortide",     "Duflor",     162, 158, 120),
        new PokemonInfo(45,  "Vileplume",  "Rafflesia",  "Giflor",     202, 190, 150),
        new PokemonInfo(46,  "Paras",      "Paras",      "Paras",      122, 120, 70),
        new PokemonInfo(47,  "Parasect",   "Parasect",   "Parasek",    162, 170, 120),
        new PokemonInfo(48,  "Venonat",    "Mimitoss",   "Bluzuk",     108, 118, 120),
        new PokemonInfo(49,  "Venomoth",   "Aéromite",   "Omot",       172, 154, 140),
        new PokemonInfo(50,  "Diglett",    "Taupiqueur", "Digda",      108, 86, 20),
        new PokemonInfo(51,  "Dugtrio",    "Triopikeur", "Digdri",     148, 140, 70),
        new PokemonInfo(52,  "Meowth",     "Miaouss",    "Mauzi",      104, 94, 80),
        new PokemonInfo(53,  "Persian",    "Persian",    "Snobilikat", 156, 146, 130),
        new PokemonInfo(54,  "Psyduck",    "Psykokwak",  "Enton",      132, 112, 100),
        new PokemonInfo(55,  "Golduck",    "Akwakwak",   "Entoron",    194, 176, 160),
        new PokemonInfo(56,  "Mankey",     "Férosinge",  "Menki",      122, 96, 80),
        new PokemonInfo(57,  "Primeape",   "Colosinge",  "Rasaff",     178, 150, 130),
        new PokemonInfo(58,  "Growlithe",  "Caninos",    "Fukano",     156, 110, 110),
        new PokemonInfo(59,  "Arcanine",   "Arcanin",    "Arkani",     230, 180, 180),
        new PokemonInfo(60,  "Poliwag",    "Ptitard",    "Quapsel",    108, 98, 80),
        new PokemonInfo(61,  "Poliwhirl",  "Têtarte",    "Quaputzi",   132, 132, 130),
        new PokemonInfo(62,  "Poliwrath",  "Tartard",    "Quappo",     180, 202, 180),
        new PokemonInfo(63,  "Abra",       "Abra",       "Abra",       110, 76, 50),
        new PokemonInfo(64,  "Kadabra",    "Kadabra",    "Kadabra",    150, 112, 80),
        new PokemonInfo(65,  "Alakazam",   "Alakazam",   "Simsala",    186, 152, 110),
        new PokemonInfo(66,  "Machop",     "Machoc",     "Machollo",   118, 96, 140),
        new PokemonInfo(67,  "Machoke",    "Machopeur",  "Maschock",   154, 144, 160),
        new PokemonInfo(68,  "Machamp",    "Mackogneur", "Machomei",   198, 180, 180),
        new PokemonInfo(69,  "Bellsprout", "Chétiflor",  "Knofensa",   158, 78, 100),
        new PokemonInfo(70,  "Weepinbell", "Boustiflor", "Ultrigaria", 190, 110, 130),
        new PokemonInfo(71,  "Victreebel", "Empiflor",   "Sarzenia",   222, 152, 160),
        new PokemonInfo(72,  "Tentacool",  "Tentacool",  "Tentacha",   106, 136, 80),
        new PokemonInfo(73,  "Tentacruel", "Tentacruel", "Tentoxa",    170, 196, 160),
        new PokemonInfo(74,  "Geodude",    "Racaillou",  "Kleinstein", 106, 118, 80),
        new PokemonInfo(75,  "Graveler",   "Gravalanch", "Georok",     142, 156, 110),
        new PokemonInfo(76,  "Golem",      "Grolem",     "Geowaz",     176, 198, 160),
        new PokemonInfo(77,  "Ponyta",     "Ponyta",     "Ponita",     168, 138, 100),
        new PokemonInfo(78,  "Rapidash",   "Galopa",     "Gallopa",    200, 170, 130),
        new PokemonInfo(79,  "Slowpoke",   "Ramoloss",   "Flegmon",    110, 110, 180),
        new PokemonInfo(80,  "Slowbro",    "Flagadoss",  "Lahmus",     184, 198, 190),
        new PokemonInfo(81,  "Magnemite",  "Magnéti",    "Magnetilo",  128, 138, 50),
        new PokemonInfo(82,  "Magneton",   "Magnéton",   "Magneton",   186, 180, 100),
        new PokemonInfo(83,  "Farfetch'd", "Canarticho", "Porenta",    138, 132, 104),
        new PokemonInfo(84,  "Doduo",      "Doduo",      "Dodu",       126, 96, 70),
        new PokemonInfo(85,  "Dodrio",     "Dodrio",     "Dodri",      182, 150, 120),
        new PokemonInfo(86,  "Seel",       "Otaria",     "Jurob",      104, 138, 130),
        new PokemonInfo(87,  "Dewgong",    "Lamantine",  "Jugong",     156, 192, 180),
        new PokemonInfo(88,  "Grimer",     "Tadmorv",    "Sleima",     124, 110, 160),
        new PokemonInfo(89,  "Muk",        "Grotadmorv", "Sleimok",    180, 188, 210),
        new PokemonInfo(90,  "Shellder",   "Kokiyas",    "Muschas",    120, 112, 60),
        new PokemonInfo(91,  "Cloyster",   "Crustabri",  "Austos",     196, 196, 100),
        new PokemonInfo(92,  "Gastly",     "Fantominus", "Nebulak",    136, 82, 60),
        new PokemonInfo(93,  "Haunter",    "Spectrum",   "Alpollo",    172, 118, 90),
        new PokemonInfo(94,  "Gengar",     "Ectoplasma", "Gengar",     204, 156, 120),
        new PokemonInfo(95,  "Onix",       "Onix",       "Onix",       90, 186, 70),
        new PokemonInfo(96,  "Drowzee",    "Soporifik",  "Traumato",   104, 140, 120),
        new PokemonInfo(97,  "Hypno",      "Hypnomade",  "Hypno",      162, 196, 170),
        new PokemonInfo(98,  "Krabby",     "Krabby",     "Krabby",     116, 110, 60),
        new PokemonInfo(99,  "Kingler",    "Krabboss",   "Kingler",    178, 168, 110),
        new PokemonInfo(100, "Voltorb",    "Voltorbe",   "Voltobal",   102, 124, 80),
        new PokemonInfo(101, "Electrode",  "Électrode",  "Lektrobal",  150, 174, 120),
        new PokemonInfo(102, "Exeggcute",  "Neouneouf",  "Owei",       110, 132, 120),
        new PokemonInfo(103, "Exeggutor",  "Noadkoko",   "Kokowei",    232, 164, 190),
        new PokemonInfo(104, "Cubone",     "Osselait",   "Tragosso",   102, 150, 100),
        new PokemonInfo(105, "Marowak",    "Ossatueur",  "Knogga",     140, 202, 120),
        new PokemonInfo(106, "Hitmonlee",  "Kicklee",    "Kicklee",    148, 172, 100),
        new PokemonInfo(107, "Hitmonchan", "Tygnon",     "Nockchan",   138, 204, 100),
        new PokemonInfo(108, "Lickitung",  "Excelangue", "Schlurp",    126, 160, 180),
        new PokemonInfo(109, "Koffing",    "Smogo",      "Smogon",     136, 142, 80),
        new PokemonInfo(110, "Weezing",    "Smogogo",    "Smogmog",    190, 198, 130),
        new PokemonInfo(111, "Rhyhorn",    "Rhinocorne", "Rihorn",     110, 116, 160),
        new PokemonInfo(112, "Rhydon",     "Rhinoféros", "Rizeros",    166, 160, 210),
        new PokemonInfo(113, "Chansey",    "Leveinard",  "Chaneira",   40, 60, 500),
        new PokemonInfo(114, "Tangela",    "Saquedeneu", "Tangela",    164, 152, 130),
        new PokemonInfo(115, "Kangaskhan", "Kangourex",  "Kangama",    142, 178, 210),
        new PokemonInfo(116, "Horsea",     "Hypotrempe", "Seeper",     122, 100, 60),
        new PokemonInfo(117, "Seadra",     "Hypocéan",   "Seemon",     176, 150, 110),
        new PokemonInfo(118, "Goldeen",    "Poissirène", "Goldini",    112, 126, 90),
        new PokemonInfo(119, "Seaking",    "Poissoroy",  "Golking",    172, 160, 160),
        new PokemonInfo(120, "Staryu",     "Stari",      "Sterndu",    130, 128, 60),
        new PokemonInfo(121, "Starmie",    "Staross",    "Starmie",    194, 192, 120),
        new PokemonInfo(122, "Mr. Mime",   "M. Mime",    "Pantimos",   154, 196, 80),
        new PokemonInfo(123, "Scyther",    "Insécateur", "Sichlor",    176, 180, 140),
        new PokemonInfo(124, "Jynx",       "Lippoutou",  "Rossana",    172, 134, 130),
        new PokemonInfo(125, "Electabuzz", "Élektek",    "Elektek",    198, 160, 130),
        new PokemonInfo(126, "Magmar",     "Magmar",     "Magmar",     214, 158, 130),
        new PokemonInfo(127, "Pinsir",     "Scarabrute", "Pinsir",     184, 186, 130),
        new PokemonInfo(128, "Tauros",     "Tauros",     "Tauros",     148, 184, 150),
        new PokemonInfo(129, "Magikarp",   "Magicarpe",  "Karpador",   42, 84, 40),
        new PokemonInfo(130, "Gyarados",   "Léviator",   "Garados",    192, 196, 190),
        new PokemonInfo(131, "Lapras",     "Lokhass",    "Lapras",     186, 190, 260),
        new PokemonInfo(132, "Ditto",      "Métamorph",  "Ditto",      110, 110, 96),
        new PokemonInfo(133, "Eevee",      "Évoli",      "Evoli",      114, 128, 110),
        new PokemonInfo(134, "Vaporeon",   "Aquali",     "Aquana",     186, 168, 260),
        new PokemonInfo(135, "Jolteon",    "Voltali",    "Blitza",     192, 174, 130),
        new PokemonInfo(136, "Flareon",    "Pyroli",     "Flamara",    238, 178, 130),
        new PokemonInfo(137, "Porygon",    "Porygon",    "Porygon",    156, 158, 130),
        new PokemonInfo(138, "Omanyte",    "Amonita",    "Amonitas",   132, 160, 70),
        new PokemonInfo(139, "Omastar",    "Amonistar",  "Amoroso",    180, 202, 140),
        new PokemonInfo(140, "Kabuto",     "Kabuto",     "Kabuto",     148, 142, 60),
        new PokemonInfo(141, "Kabutops",   "Kabutops",   "Kabutops",   190, 190, 120),
        new PokemonInfo(142, "Aerodactyl", "Ptéra",      "Aerodactyl", 182, 162, 160),
        new PokemonInfo(143, "Snorlax",    "Ronflex",    "Relaxo",     180, 180, 320),
        new PokemonInfo(144, "Articuno",   "Artikodin",  "Arktos",     198, 242, 180),
        new PokemonInfo(145, "Zapdos",     "Électhor",   "Zapdos",     232, 194, 180),
        new PokemonInfo(146, "Moltres",    "Sulfura",    "Lavados",    242, 194, 180),
        new PokemonInfo(147, "Dratini",    "Minidraco",  "Dratini",    128, 110, 82),
        new PokemonInfo(148, "Dragonair",  "Draco",      "Dragonir",   170, 152, 122),
        new PokemonInfo(149, "Dragonite",  "Dracolosse", "Dragoran",   250, 212, 182),
        new PokemonInfo(150, "Mewtwo",     "Mewtwo",     "Mewtu",      284, 202, 212),
        new PokemonInfo(151, "Mew",        "Mew",        "Mew",        220, 220, 200),
    };
}
























































































































































