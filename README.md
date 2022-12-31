# Strid

Un jeu de combat de cartes à deux joueurs (humain vs COM), réalisé sous Unity avec Firebase.
## Installation

* Télécharger Strid.apk (TODO: insérer lien plus tard) sur votre téléphone
* Trouver et lancer le fichier APK
## Comment jouer

*Note : Les captures d'écran ont été réalisées en modifiant l'application directement.*

Après un léger temps d'attente (nécessaire pour importer les cartes), vous arrivez à l'écran principal.

![Écran de chargement et Menu principal](https://github.com/CreeperStone72/Strid/blob/main/Assets/Screenshots/Loading-Menu.gif)

Pour lancer une partie, il suffit de cliquer sur START.

### Début de partie

En début de chaque round, chaque joueur reçoit un certain nombre de cartes.

| Round | Cartes distribuées |
|:-----:|:------------------:|
| 1     | 11                 |
| 2     |  2                 |
| 3     |  1                 |

Pour le premier round, le joueur humain joue en premier. Pour les rounds suivants, le vainqueur de la manche précédente joue en premier.

![Début de partie](https://github.com/CreeperStone72/Strid/blob/main/Assets/Screenshots/GameStart.png)

### But du jeu

Chaque joueur pose tour à tour une carte, augmentant petit à petit la force de leur armée respective. Quand un joueur n'a plus de cartes ou qu'il estime en avoir posé suffisamment, il peut passer son tour.

Une fois que les deux joueurs ont passé leur tour, on compare la puissance de combat de leur armée. L'armée la plus puissante gagne le round.

Enfin, toutes les cartes actuellement en jeu sont envoyées au cimetière (défausse). Le round suivant peut commencer.

### Arène de combat

Chaque joueur possède trois lignes de combat :
* Mêlée (chevaliers, infantrie...)
* Portée (archers, arbalétriers...)
* Siège (trébuchets, catapultes...)

![Lignes de combat](https://github.com/CreeperStone72/Strid/blob/main/Assets/Screenshots/Lines.png)

Chaque carte possède une ligne attitrée, un rôle, ainsi qu'une puissance de combat.

***Règle de base** - Valable pour chaque ligne, sauf mention contraire*
* ♥ Cœurs (Hearts) = N'importe quelle ligne
* ♠ Piques (Spades) = Mêlée
* ♦ Carreaux (Diamonds) = Portée
* ♣ Trèfle (Clubs) = Siège

| Rôle                     | Utilisation                                                                     | Puissance de combat     | Exemple (jeu de 54 cartes classiques)  |
|:------------------------:|---------------------------------------------------------------------------------|:-----------------------:|----------------------------------------|
| Unité de combat          | Poser sur la ligne attitrée                                                     | Valeur indiquée         | 2, 3, 4, 5, 6, 7, 8, 9, 10 (♠ - ♦ - ♣) |
| *Espion*                 | Poser sur la ligne attitrée adverse, piocher deux cartes                        | 10                      | ♥J, ♠J, ♦J, ♣J                         |
| Renforcement (unité)     | Ajoute sa puissance à une Unité de combat de la ligne attitrée                  | Valeur indiquée         | ♥2, ♥3, ♥4, ♥5, ♥6, ♥7, ♥8, ♥9, ♥10    |
| Renforcement (ligne)     | Double la puissance de combat de la ligne attitrée                              | 0 (x2 pour la ligne)    | ♥K, ♠K, ♦K, ♣K                         |
| Météo (débuff)           | Nullifie la puissance de combat de la ligne attitrée (affecte les deux joueurs) | 0 (x0 pour la ligne)    | ♠A, ♦A, ♣A                             |
| Météo (beau temps)       | Annule toutes les cartes Météo (débuff) en jeu                                  | 0                       | ♥A                                     |
| Leurre                   | Remplace une carte en jeu qui retourne dans la main du joueur                   | 0                       | Joker rouge, Joker noir                |
| *Spécial (Résurrection)* | Joue une carte de la défausse                                                   | Défaussée immédiatement | ♥Q                                     |
| *Spécial (Destruction)*  | Détruit n'importe quelle Unité de combat en jeu                                 | Défaussée immédiatement | ♠Q                                     |
| *Spécial (Tirage bonus)* | Tire une carte, défausse une carte                                              | Défaussée immédiatement | ♦Q                                     |
| *Spécial (Terre brûlée)* | Défausse la(es) carte(s) avec la puissance de combat la plus élevée             | Défaussée immédiatement | ♣Q                                     |

## Stockage BDD (Firebase)

Firebase permet une intégration facile avec Unity, et les deux solutions adoptées permettent de répondre aux deux problèmes majeurs.

### Informations cartes (Realtime Database)

Le premier problème consiste à stocker les informations de chaque carte. Pour ce faire, les informations de la carte sont stockées dans l'objet JSON suivant :

**Valeurs**
| Parameter     | Type     | Description                         |
|:--------------|:---------|:------------------------------------|
| `cardId`      | `int`    | Identifiant                         |
| `title`       | `string` | Nom de la carte                     |
| `line`        | `int`    | Ligne attitrée                      |
| `type`        | `int`    | Type de la carte                    |
| `combatPower` | `int`    | Puissance de combat                 |

En plus des données des cartes, la table `cards` comporte également un attribut `count` (`int`) contenant le nombre de cartes stockées.

**Correspondances**
| N° | Ligne     |
|:--:|:----------|
|  0 | Mêlée     |
|  1 | Portée    |
|  2 | Siège     |
|  3 | N'importe |

| N° | Type de carte        |
|:--:|:---------------------|
|  0 | Unité de combat      |
|  1 | Espion               |
|  2 | Renforcement (unité) |
|  3 | Renforcement (ligne) |
|  4 | Météo                |
|  5 | Leurre               |
|  6 | Spécial              |

### Illustrations cartes (Storage)

Les illustrations des cartes sont stockées dans une autre BDD de Firebase. La BDD contient les octets de chaque image, identifiée par l'attribut `title` de la carte correspondante.

⚠️ **NOTE IMPORTANTE :**
Firebase impose un quota de 10 Gio de données transférées par jour. À cause de cela, le nombre de parties quotidienne est limité.  
Quand le quota est dépassé, il est toujours possible de jouer, mais les cartes n'ont plus d'illustration.
Le quota est réinitialisé à **9h UTC+1**.

## Problèmes connus

#### **Cycle de jeu incomplet**

Actuellement, il n'est pas possible de jouer une partie complète de Strid.

Un bug survient quand le joueur essaie de poser une carte : celle-ci n'est pas déplacée vers la ligne correspondante, mais le tour passe quand même au joueur COM, qui n'agit pas.  
Après plusieurs jours passés sur ce problème, je n'ai pas réussi à en déterminer l'origine et suis contraint d'abandonner, faute de temps.

Si ce bug venait à être réglé, les seuls problèmes potentiels proviendraient du bouton permettant de marquer la fin du tour et/ou de la gestion de fin de round et de partie.

#### **Quota insuffisant**

Comme mentionné ci-dessus, le quota de Firebase fait que l'on peut se retrouver avec des cartes sans illustrations si l'on joue trop de parties dans la même journée. Certains jours, j'ai atteint le quota en seulement deux heures de test.  
Le champ de description permet quand même de savoir quelles cartes le joueur a en main, mais l'information est perdue au moment où celle-ci est jouée ou défaussée.

Il y a trois solutions possibles :
1. Payer pour augmenter le quota
2. Stocker des textures compressées (solution temporaire, elle ne fait qu'augmenter le nombre de parties quotidiennes autorisées, mais n'enlève pas la limite)
3. Stocker les illustrations localement (élimine la limite quotidienne, mais augmente la taille de l'application)
## Améliorations possibles

#### Gameplay
* Cartes Espion (actuel : comportement d'une Unité de combat)
* Cartes Spéciales (actuel : comportement d'une Unité de combat)
* Plus de cartes
    * Système de deck
    * Autres cartes Spéciales
* Mode JvJ
* Possibilité de voir l'armée adverse

#### Esthétique
* Audio (musique et effets sonores)
* Cartes plus travaillées
    * Meilleures illustrations
    * Informations visbles sur la carte, même sans illustration
* Icônes pour le Renforcement (ligne) et la Météo
* Image d'arrière-plan

