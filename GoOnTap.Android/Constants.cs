﻿using System;
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
    public static double GetLevelAngle(double level, int playerLevel) => (Constants.CPMultipliers[level] - 0.094) * 180.0 / (Constants.CPMultipliers[Math.Min(playerLevel + 1.5, 40.5)] - 0.094);
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
        new PokemonInfo(1,   "Bulbasaur",  "Bulbizarre", "Bisasam",    118, 118, 90),
        new PokemonInfo(2,   "Ivysaur",    "Herbizarre", "Bisaknosp",  151, 151, 120),
        new PokemonInfo(3,   "Venusaur",   "Florizarre", "Bisaflor",   198, 198, 160),
        new PokemonInfo(4,   "Charmander", "Salamèche",  "Glumanda",   116, 96, 78),
        new PokemonInfo(5,   "Charmeleon", "Reptincel",  "Glutexo",    158, 129, 116),
        new PokemonInfo(6,   "Charizard",  "Dracaufeu",  "Glurak",     223, 176, 156),
        new PokemonInfo(7,   "Squirtle",   "Carapuce",   "Schiggy",    94, 122, 88),
        new PokemonInfo(8,   "Wartortle",  "Carabaffe",  "Schillok",   126, 155, 118),
        new PokemonInfo(9,   "Blastoise",  "Tortank",    "Turtok",     171, 210, 158),
        new PokemonInfo(10,  "Caterpie",   "Chenipan",   "Raupy",      55, 62, 90),
        new PokemonInfo(11,  "Metapod",    "Chrysacier", "Safcon",     45, 94, 100),
        new PokemonInfo(12,  "Butterfree", "Papilusion", "Smettbo",    167, 151, 120),
        new PokemonInfo(13,  "Weedle",     "Aspicot",    "Hornliu",    63, 55, 80),
        new PokemonInfo(14,  "Kakuna",     "Conconfort", "Kokuna",     46, 86, 90),
        new PokemonInfo(15,  "Beedrill",   "Dardargnan", "Bibor",      169, 150, 130),
        new PokemonInfo(16,  "Pidgey",     "Roucool",    "Taubsi",     85, 76, 80),
        new PokemonInfo(17,  "Pidgeotto",  "Roucoups",   "Tauboga",    117, 108, 126),
        new PokemonInfo(18,  "Pidgeot",    "Roucarnage", "Tauboss",    166, 157, 166),
        new PokemonInfo(19,  "Rattata",    "Rattata",    "Rattfratz",  103, 70, 60),
        new PokemonInfo(20,  "Raticate",   "Rattatac",   "Rattikarl",  161, 144, 110),
        new PokemonInfo(21,  "Spearow",    "Piafabec",   "Habitak",    112, 61, 80),
        new PokemonInfo(22,  "Fearow",     "Rapasdepic", "Ibitak",     182, 135, 130),
        new PokemonInfo(23,  "Ekans",      "Abo",        "Rettan",     110, 102, 70),
        new PokemonInfo(24,  "Arbok",      "Arbok",      "Arbok",      167, 158, 120),
        new PokemonInfo(25,  "Pikachu",    "Pikachu",    "Pikachu",    112, 101, 70),
        new PokemonInfo(26,  "Raichu",     "Raichu",     "Raichu",     193, 165, 120),
        new PokemonInfo(27,  "Sandshrew",  "Sabelette",  "Sandan",     126, 145, 100),
        new PokemonInfo(28,  "Sandslash",  "Sablaireau", "Sandamer",   182, 202, 150),
        new PokemonInfo(29,  "Nidoran♀",   "Nidoran♀",   "Nidoran♀",   86, 94, 110),
        new PokemonInfo(30,  "Nidorina",   "Nidorina",   "Nidorina",   117, 126, 140),
        new PokemonInfo(31,  "Nidoqueen",  "Nidoqueen",  "Nidoqueen",  180, 174, 180),
        new PokemonInfo(32,  "Nidoran♂",   "Nidoran♂",   "Nidoran♂",   105, 76, 92),
        new PokemonInfo(33,  "Nidorino",   "Nidorino",   "Nidorino",   137, 112, 122),
        new PokemonInfo(34,  "Nidoking",   "Nidoking",   "Nidoking",   204, 157, 162),
        new PokemonInfo(35,  "Clefairy",   "Mélofée",    "Piepi",      107, 116, 140),
        new PokemonInfo(36,  "Clefable",   "Mélodelfe",  "Pixi",       178, 171, 190),
        new PokemonInfo(37,  "Vulpix",     "Goupix",     "Vulpix",     96, 122, 76),
        new PokemonInfo(38,  "Ninetales",  "Feunard",    "Vulnona",    169, 204, 146),
        new PokemonInfo(39,  "Jigglypuff", "Rondoudou",  "Pummeluff",  80, 44, 230),
        new PokemonInfo(40,  "Wigglytuff", "Grodoudou",  "Knuddeluff", 156, 93, 280),
        new PokemonInfo(41,  "Zubat",      "Nosferapti", "Zubat",      83, 76, 80),
        new PokemonInfo(42,  "Golbat",     "Nosferalto", "Golbat",     161, 153, 150),
        new PokemonInfo(43,  "Oddish",     "Mystherbe",  "Myrapla",    131, 116, 90),
        new PokemonInfo(44,  "Gloom",      "Ortide",     "Duflor",     153, 139, 120),
        new PokemonInfo(45,  "Vileplume",  "Rafflesia",  "Giflor",     202, 170, 150),
        new PokemonInfo(46,  "Paras",      "Paras",      "Paras",      121, 99, 70),
        new PokemonInfo(47,  "Parasect",   "Parasect",   "Parasek",    165, 146, 120),
        new PokemonInfo(48,  "Venonat",    "Mimitoss",   "Bluzuk",     100, 102, 120),
        new PokemonInfo(49,  "Venomoth",   "Aéromite",   "Omot",       179, 150, 140),
        new PokemonInfo(50,  "Diglett",    "Taupiqueur", "Digda",      109, 88, 20),
        new PokemonInfo(51,  "Dugtrio",    "Triopikeur", "Digdri",     167, 147, 70),
        new PokemonInfo(52,  "Meowth",     "Miaouss",    "Mauzi",      92, 81, 80),
        new PokemonInfo(53,  "Persian",    "Persian",    "Snobilikat", 150, 139, 130),
        new PokemonInfo(54,  "Psyduck",    "Psykokwak",  "Enton",      122, 96, 100),
        new PokemonInfo(55,  "Golduck",    "Akwakwak",   "Entoron",    191, 163, 160),
        new PokemonInfo(56,  "Mankey",     "Férosinge",  "Menki",      148, 87, 80),
        new PokemonInfo(57,  "Primeape",   "Colosinge",  "Rasaff",     207, 144, 130),
        new PokemonInfo(58,  "Growlithe",  "Caninos",    "Fukano",     136, 96, 110),
        new PokemonInfo(59,  "Arcanine",   "Arcanin",    "Arkani",     227, 166, 180),
        new PokemonInfo(60,  "Poliwag",    "Ptitard",    "Quapsel",    101, 82, 80),
        new PokemonInfo(61,  "Poliwhirl",  "Têtarte",    "Quaputzi",   130, 130, 130),
        new PokemonInfo(62,  "Poliwrath",  "Tartard",    "Quappo",     182, 187, 180),
        new PokemonInfo(63,  "Abra",       "Abra",       "Abra",       195, 103, 50),
        new PokemonInfo(64,  "Kadabra",    "Kadabra",    "Kadabra",    232, 138, 80),
        new PokemonInfo(65,  "Alakazam",   "Alakazam",   "Simsala",    271, 194, 110),
        new PokemonInfo(66,  "Machop",     "Machoc",     "Machollo",   137, 88, 140),
        new PokemonInfo(67,  "Machoke",    "Machopeur",  "Maschock",   177, 130, 160),
        new PokemonInfo(68,  "Machamp",    "Mackogneur", "Machomei",   234, 162, 180),
        new PokemonInfo(69,  "Bellsprout", "Chétiflor",  "Knofensa",   139, 64, 100),
        new PokemonInfo(70,  "Weepinbell", "Boustiflor", "Ultrigaria", 172, 95, 130),
        new PokemonInfo(71,  "Victreebel", "Empiflor",   "Sarzenia",   207, 138, 160),
        new PokemonInfo(72,  "Tentacool",  "Tentacool",  "Tentacha",   97, 182, 80),
        new PokemonInfo(73,  "Tentacruel", "Tentacruel", "Tentoxa",    166, 237, 160),
        new PokemonInfo(74,  "Geodude",    "Racaillou",  "Kleinstein", 132, 163, 80),
        new PokemonInfo(75,  "Graveler",   "Gravalanch", "Georok",     164, 196, 110),
        new PokemonInfo(76,  "Golem",      "Grolem",     "Geowaz",     211, 229, 160),
        new PokemonInfo(77,  "Ponyta",     "Ponyta",     "Ponita",     170, 132, 100),
        new PokemonInfo(78,  "Rapidash",   "Galopa",     "Gallopa",    207, 167, 130),
        new PokemonInfo(79,  "Slowpoke",   "Ramoloss",   "Flegmon",    109, 109, 180),
        new PokemonInfo(80,  "Slowbro",    "Flagadoss",  "Lahmus",     177, 194, 190),
        new PokemonInfo(81,  "Magnemite",  "Magnéti",    "Magnetilo",  165, 128, 50),
        new PokemonInfo(82,  "Magneton",   "Magnéton",   "Magneton",   223, 182, 100),
        new PokemonInfo(83,  "Farfetch'd", "Canarticho", "Porenta",    124, 118, 104),
        new PokemonInfo(84,  "Doduo",      "Doduo",      "Dodu",       158, 88, 70),
        new PokemonInfo(85,  "Dodrio",     "Dodrio",     "Dodri",      218, 145, 120),
        new PokemonInfo(86,  "Seel",       "Otaria",     "Jurob",      85, 128, 130),
        new PokemonInfo(87,  "Dewgong",    "Lamantine",  "Jugong",     139, 184, 180),
        new PokemonInfo(88,  "Grimer",     "Tadmorv",    "Sleima",     135, 90, 160),
        new PokemonInfo(89,  "Muk",        "Grotadmorv", "Sleimok",    190, 184, 210),
        new PokemonInfo(90,  "Shellder",   "Kokiyas",    "Muschas",    116, 168, 60),
        new PokemonInfo(91,  "Cloyster",   "Crustabri",  "Austos",     186, 323, 100),
        new PokemonInfo(92,  "Gastly",     "Fantominus", "Nebulak",    186, 70, 60),
        new PokemonInfo(93,  "Haunter",    "Spectrum",   "Alpollo",    223, 112, 90),
        new PokemonInfo(94,  "Gengar",     "Ectoplasma", "Gengar",     261, 156, 120),
        new PokemonInfo(95,  "Onix",       "Onix",       "Onix",       85, 288, 70),
        new PokemonInfo(96,  "Drowzee",    "Soporifik",  "Traumato",   89, 158, 120),
        new PokemonInfo(97,  "Hypno",      "Hypnomade",  "Hypno",      144, 215, 170),
        new PokemonInfo(98,  "Krabby",     "Krabby",     "Krabby",     181, 156, 60),
        new PokemonInfo(99,  "Kingler",    "Krabboss",   "Kingler",    240, 214, 110),
        new PokemonInfo(100, "Voltorb",    "Voltorbe",   "Voltobal",   109, 114, 80),
        new PokemonInfo(101, "Electrode",  "Électrode",  "Lektrobal",  173, 179, 120),
        new PokemonInfo(102, "Exeggcute",  "Neouneouf",  "Owei",       107, 140, 120),
        new PokemonInfo(103, "Exeggutor",  "Noadkoko",   "Kokowei",    233, 158, 190),
        new PokemonInfo(104, "Cubone",     "Osselait",   "Tragosso",   90, 165, 100),
        new PokemonInfo(105, "Marowak",    "Ossatueur",  "Knogga",     144, 200, 120),
        new PokemonInfo(106, "Hitmonlee",  "Kicklee",    "Kicklee",    224, 211, 100),
        new PokemonInfo(107, "Hitmonchan", "Tygnon",     "Nockchan",   193, 212, 100),
        new PokemonInfo(108, "Lickitung",  "Excelangue", "Schlurp",    108, 137, 180),
        new PokemonInfo(109, "Koffing",    "Smogo",      "Smogon",     119, 164, 80),
        new PokemonInfo(110, "Weezing",    "Smogogo",    "Smogmog",    174, 221, 130),
        new PokemonInfo(111, "Rhyhorn",    "Rhinocorne", "Rihorn",     140, 157, 160),
        new PokemonInfo(112, "Rhydon",     "Rhinoféros", "Rizeros",    222, 206, 210),
        new PokemonInfo(113, "Chansey",    "Leveinard",  "Chaneira",   60, 176, 500),
        new PokemonInfo(114, "Tangela",    "Saquedeneu", "Tangela",    183, 205, 130),
        new PokemonInfo(115, "Kangaskhan", "Kangourex",  "Kangama",    181, 165, 210),
        new PokemonInfo(116, "Horsea",     "Hypotrempe", "Seeper",     129, 125, 60),
        new PokemonInfo(117, "Seadra",     "Hypocéan",   "Seemon",     187, 182, 110),
        new PokemonInfo(118, "Goldeen",    "Poissirène", "Goldini",    123, 115, 90),
        new PokemonInfo(119, "Seaking",    "Poissoroy",  "Golking",    175, 154, 160),
        new PokemonInfo(120, "Staryu",     "Stari",      "Sterndu",    137, 112, 60),
        new PokemonInfo(121, "Starmie",    "Staross",    "Starmie",    210, 184, 120),
        new PokemonInfo(122, "Mr. Mime",   "M. Mime",    "Pantimos",   192, 233, 80),
        new PokemonInfo(123, "Scyther",    "Insécateur", "Sichlor",    218, 170, 140),
        new PokemonInfo(124, "Jynx",       "Lippoutou",  "Rossana",    223, 182, 130),
        new PokemonInfo(125, "Electabuzz", "Élektek",    "Elektek",    198, 173, 130),
        new PokemonInfo(126, "Magmar",     "Magmar",     "Magmar",     206, 169, 130),
        new PokemonInfo(127, "Pinsir",     "Scarabrute", "Pinsir",     238, 197, 130),
        new PokemonInfo(128, "Tauros",     "Tauros",     "Tauros",     198, 197, 150),
        new PokemonInfo(129, "Magikarp",   "Magicarpe",  "Karpador",   29, 102, 40),
        new PokemonInfo(130, "Gyarados",   "Léviator",   "Garados",    237, 197, 190),
        new PokemonInfo(131, "Lapras",     "Lokhass",    "Lapras",     186, 190, 260),
        new PokemonInfo(132, "Ditto",      "Métamorph",  "Ditto",      91, 91, 96),
        new PokemonInfo(133, "Eevee",      "Évoli",      "Evoli",      104, 121, 110),
        new PokemonInfo(134, "Vaporeon",   "Aquali",     "Aquana",     205, 177, 260),
        new PokemonInfo(135, "Jolteon",    "Voltali",    "Blitza",     232, 201, 130),
        new PokemonInfo(136, "Flareon",    "Pyroli",     "Flamara",    246, 204, 130),
        new PokemonInfo(137, "Porygon",    "Porygon",    "Porygon",    153, 139, 130),
        new PokemonInfo(138, "Omanyte",    "Amonita",    "Amonitas",   155, 174, 70),
        new PokemonInfo(139, "Omastar",    "Amonistar",  "Amoroso",    207, 227, 140),
        new PokemonInfo(140, "Kabuto",     "Kabuto",     "Kabuto",     148, 162, 60),
        new PokemonInfo(141, "Kabutops",   "Kabutops",   "Kabutops",   220, 203, 120),
        new PokemonInfo(142, "Aerodactyl", "Ptéra",      "Aerodactyl", 221, 164, 160),
        new PokemonInfo(143, "Snorlax",    "Ronflex",    "Relaxo",     190, 190, 320),
        new PokemonInfo(144, "Articuno",   "Artikodin",  "Arktos",     192, 249, 180),
        new PokemonInfo(145, "Zapdos",     "Électhor",   "Zapdos",     253, 188, 180),
        new PokemonInfo(146, "Moltres",    "Sulfura",    "Lavados",    251, 184, 180),
        new PokemonInfo(147, "Dratini",    "Minidraco",  "Dratini",    119, 94, 82),
        new PokemonInfo(148, "Dragonair",  "Draco",      "Dragonir",   163, 138, 122),
        new PokemonInfo(149, "Dragonite",  "Dracolosse", "Dragoran",   263, 201, 182),
        new PokemonInfo(150, "Mewtwo",     "Mewtwo",     "Mewtu",      330, 200, 212),
        new PokemonInfo(151, "Mew",        "Mew",        "Mew",        210, 210, 200),
        new PokemonInfo(152, "Chikorita", "Germignon", "Endivie", 92, 122, 90),
        new PokemonInfo(153, "Bayleef", "Macronium", "Lorblatt", 122, 155, 120),
        new PokemonInfo(154, "Meganium", "Méganium", "Meganie", 168, 202, 160),
        new PokemonInfo(155, "Cyndaquil", "Héricendre", "Feurigel", 116, 96, 78),
        new PokemonInfo(156, "Quilava", "Feurisson", "Igelavar", 158, 129, 116),
        new PokemonInfo(157, "Typhlosion", "Typhlosion", "Tornupto", 223, 176, 156),
        new PokemonInfo(158, "Totodile", "Kaiminus", "Karnimani", 117, 116, 100),
        new PokemonInfo(159, "Croconaw", "Crocrodil", "Tyracroc", 150, 151, 130),
        new PokemonInfo(160, "Feraligatr", "Aligatueur", "Impergator", 205, 197, 170),
        new PokemonInfo(161, "Sentret", "Fouinette", "Wiesor", 79, 77, 70),
        new PokemonInfo(162, "Furret", "Fouinar", "Wiesenior", 148, 130, 170),
        new PokemonInfo(163, "Hoothoot", "Hoothoot", "Hoothoot", 67, 101, 120),
        new PokemonInfo(164, "Noctowl", "Noarfang", "Noctuh", 145, 179, 200),
        new PokemonInfo(165, "Ledyba", "Coxy", "Ledyba", 72, 142, 80),
        new PokemonInfo(166, "Ledian", "Coxyclaque", "Ledian", 107, 209, 110),
        new PokemonInfo(167, "Spinarak", "Mimigal", "Webarak", 105, 73, 80),
        new PokemonInfo(168, "Ariados", "Migalos", "Ariados", 161, 128, 140),
        new PokemonInfo(169, "Crobat", "Nostenfer", "Iksbat", 194, 178, 170),
        new PokemonInfo(170, "Chinchou", "Loupio", "Lampi", 106, 106, 150),
        new PokemonInfo(171, "Lanturn", "Lanturn", "Lanturn", 146, 146, 250),
        new PokemonInfo(172, "Pichu", "Pichu", "Pichu", 77, 63, 40),
        new PokemonInfo(173, "Cleffa", "Mélo", "Pii", 75, 91, 100),
        new PokemonInfo(174, "Igglybuff", "Toudoudou", "Fluffeluff", 69, 34, 180),
        new PokemonInfo(175, "Togepi", "Togepi", "Togepi", 67, 116, 70),
        new PokemonInfo(176, "Togetic", "Togetic", "Togetic", 140, 191, 110),
        new PokemonInfo(177, "Natu", "Natu", "Natu", 134, 89, 80),
        new PokemonInfo(178, "Xatu", "Xatu", "Xatu", 192, 146, 130),
        new PokemonInfo(179, "Mareep", "Wattouat", "Voltilamm", 114, 82, 110),
        new PokemonInfo(180, "Flaaffy", "Lainergie", "Waaty", 145, 112, 140),
        new PokemonInfo(181, "Ampharos", "Pharamp", "Ampharos", 211, 172, 180),
        new PokemonInfo(182, "Bellossom", "Joliflor", "Blubella", 169, 189, 150),
        new PokemonInfo(183, "Marill", "Marill", "Marill", 37, 93, 140),
        new PokemonInfo(184, "Azumarill", "Azumarill", "Azumarill", 112, 152, 200),
        new PokemonInfo(185, "Sudowoodo", "Simularbre", "Mogelbaum", 167, 198, 140),
        new PokemonInfo(186, "Politoed", "Tarpaud", "Quaxo", 174, 192, 180),
        new PokemonInfo(187, "Hoppip", "Granivol", "Hoppspross", 67, 101, 70),
        new PokemonInfo(188, "Skiploom", "Floravol", "Hubelupf", 91, 127, 110),
        new PokemonInfo(189, "Jumpluff", "Cotovol", "Papungha", 118, 197, 150),
        new PokemonInfo(190, "Aipom", "Capumain", "Griffel", 136, 112, 110),
        new PokemonInfo(191, "Sunkern", "Tournegrin", "Sonnkern", 55, 55, 60),
        new PokemonInfo(192, "Sunflora", "Héliatronc", "Sonnflora", 185, 148, 150),
        new PokemonInfo(193, "Yanma", "Yanma", "Yanma", 154, 94, 130),
        new PokemonInfo(194, "Wooper", "Axoloto", "Felino", 75, 75, 110),
        new PokemonInfo(195, "Quagsire", "Maraiste", "Morlord", 152, 152, 190),
        new PokemonInfo(196, "Espeon", "Mentali", "Psiana", 261, 194, 130),
        new PokemonInfo(197, "Umbreon", "Noctali", "Nachtara", 126, 250, 190),
        new PokemonInfo(198, "Murkrow", "Cornèbre", "Kramurx", 175, 87, 120),
        new PokemonInfo(199, "Slowking", "Roigada", "Laschoking", 177, 194, 190),
        new PokemonInfo(200, "Misdreavus", "Feuforêve", "Traunfugil", 167, 167, 120),
        new PokemonInfo(201, "Unown", "Zarbi", "Icognito", 136, 91, 96),
        new PokemonInfo(202, "Wobbuffet", "Qulbutoké", "Woingenau", 60, 106, 380),
        new PokemonInfo(203, "Girafarig", "Girafarig", "Girafarig", 182, 133, 140),
        new PokemonInfo(204, "Pineco", "Pomdepik", "Tannza", 108, 146, 100),
        new PokemonInfo(205, "Forretress", "Foretress", "Forstellka", 161, 242, 150),
        new PokemonInfo(206, "Dunsparce", "Insolourdo", "Dummisel", 131, 131, 200),
        new PokemonInfo(207, "Gligar", "Scorplane", "Skorgla", 143, 204, 130),
        new PokemonInfo(208, "Steelix", "Steelix", "Stahlos", 148, 333, 150),
        new PokemonInfo(209, "Snubbull", "Snubbull", "Snubbull", 137, 89, 120),
        new PokemonInfo(210, "Granbull", "Granbull", "Granbull", 212, 137, 180),
        new PokemonInfo(211, "Qwilfish", "Qwilfish", "Baldorfish", 184, 148, 130),
        new PokemonInfo(212, "Scizor", "Cizayox", "Scherox", 236, 191, 140),
        new PokemonInfo(213, "Shuckle", "Caratroc", "Pottrott", 17, 396, 40),
        new PokemonInfo(214, "Heracross", "Scarhino", "Skaraborn", 234, 189, 160),
        new PokemonInfo(215, "Sneasel", "Farfuret", "Sniebel", 189, 157, 110),
        new PokemonInfo(216, "Teddiursa", "Teddiursa", "Teddiursa", 142, 93, 120),
        new PokemonInfo(217, "Ursaring", "Ursaring", "Ursaring", 236, 144, 180),
        new PokemonInfo(218, "Slugma", "Limagma", "Schneckmag", 118, 71, 80),
        new PokemonInfo(219, "Magcargo", "Volcaropod", "Magcargo", 139, 209, 100),
        new PokemonInfo(220, "Swinub", "Marcacrin", "Quiekel", 90, 74, 100),
        new PokemonInfo(221, "Piloswine", "Cochignon", "Keifel", 181, 147, 200),
        new PokemonInfo(222, "Corsola", "Corayon", "Corasonn", 118, 156, 110),
        new PokemonInfo(223, "Remoraid", "Rémoraid", "Remoraid", 127, 69, 70),
        new PokemonInfo(224, "Octillery", "Octillery", "Octillery", 197, 141, 150),
        new PokemonInfo(225, "Delibird", "Cadoizo", "Botogel", 128, 90, 90),
        new PokemonInfo(226, "Mantine", "Démanta", "Mantax", 149, 260, 130),
        new PokemonInfo(227, "Skarmory", "Airmure", "Panzaeron", 149, 260, 130),
        new PokemonInfo(228, "Houndour", "Malosse", "Hunduster", 152, 93, 90),
        new PokemonInfo(229, "Houndoom", "Démolosse", "Hundemon", 224, 159, 150),
        new PokemonInfo(230, "Kingdra", "Hyporoi", "Seedraking", 194, 194, 150),
        new PokemonInfo(231, "Phanpy", "Phanpy", "Phanpy", 107, 107, 180),
        new PokemonInfo(232, "Donphan", "Donphan", "Donphan", 214, 214, 180),
        new PokemonInfo(233, "Porygon2", "Porygon2", "Porygon2", 198, 183, 170),
        new PokemonInfo(234, "Stantler", "Cerfrousse", "Damhirplex", 192, 132, 146),
        new PokemonInfo(235, "Smeargle", "Queulorior", "Farbeagle", 40, 88, 110),
        new PokemonInfo(236, "Tyrogue", "Debugant", "Rabauz", 64, 64, 70),
        new PokemonInfo(237, "Hitmontop", "Kapoera", "Kapoera", 173, 214, 100),
        new PokemonInfo(238, "Smoochum", "Lippouti", "Kussilla", 153, 116, 90),
        new PokemonInfo(239, "Elekid", "Élekid", "Elekid", 135, 110, 90),
        new PokemonInfo(240, "Magby", "Magby", "Magby", 151, 108, 90),
        new PokemonInfo(241, "Miltank", "Écrémeuh", "Miltank", 158, 211, 190),
        new PokemonInfo(242, "Blissey", "Leuphorie", "Heiteira", 129, 229, 510),
        new PokemonInfo(243, "Raikou", "Raikou", "Raikou", 241, 210, 180),
        new PokemonInfo(244, "Entei", "Entei", "Entei", 235, 176, 230),
        new PokemonInfo(245, "Suicune", "Suicune", "Suicune", 180, 235, 200),
        new PokemonInfo(246, "Larvitar", "Embrylex", "Larvitar", 115, 93, 100),
        new PokemonInfo(247, "Pupitar", "Ymphect", "Pupitar", 155, 133, 140),
        new PokemonInfo(248, "Tyranitar", "Tyranocif", "Despotar", 251, 212, 200),
        new PokemonInfo(249, "Lugia", "Lugia", "Lugia", 193, 323, 212),
        new PokemonInfo(250, "Ho-Oh", "Ho-Oh", "Ho-Oh", 263, 301, 212),
        new PokemonInfo(251, "Celebi", "Celebi", "Celebi", 210, 210, 200),
    };
}
























































































































































