# ProjectPilot Lite

[![CI](https://github.com/yonilancry/project-pilot-lite/actions/workflows/ci.yml/badge.svg)](https://github.com/yonilancry/project-pilot-lite/actions/workflows/ci.yml)

Outil interne léger de suivi de projets techniques : une **API REST ASP.NET Core** (persistance
via Entity Framework Core / SQLite) et un **client de bureau MVVM** qui la consomme. L'outil
fonctionne entièrement en local.

Le client existe en **deux têtes** partageant la même logique :

- **WPF** (`net10.0-windows`) — le client de bureau Windows.
- **Avalonia** (`net10.0`) — le même client, cross-platform (macOS / Linux / Windows), pratique
  pour développer et démontrer l'application sur un poste non-Windows.

Les deux consomment l'API de façon identique : seules les vues diffèrent, les ViewModels et le
service d'appel API sont **mutualisés** dans `ProjectPilotLite.Client.Shared`.

## Prérequis

- **SDK .NET 10** ([télécharger](https://dotnet.microsoft.com/download)).
- **Windows** pour compiler et lancer le client **WPF** (WPF est propre à Windows).
- Le client **Avalonia** et l'**API** fonctionnent sur Windows, macOS et Linux.

## Structure de la solution

```
ProjectPilotLite.sln
├── ProjectPilotLite.Core            Entités (Projet, Tâche, Livrable), enums, DTOs
├── ProjectPilotLite.Api             API REST, EF Core, SQLite, migrations, tableau de bord
├── ProjectPilotLite.Client.Shared   ViewModels (MVVM) + service ApiClient (HttpClient)
├── ProjectPilotLite.Wpf             Vues WPF (Windows)
└── ProjectPilotLite.Avalonia        Vues Avalonia (cross-platform)
ProjectPilotLite.Local.slnf          Filtre de solution : tout sauf WPF (build sur macOS/Linux)
```

Le client ne touche **jamais** la base de données directement : il passe systématiquement par
l'API en HTTP/JSON.

## Lancer l'API

```bash
dotnet run --project ProjectPilotLite.Api
```

- L'API démarre sur **http://localhost:5123**.
- La base SQLite (`projectpilot.db`) est **créée et migrée automatiquement** au premier démarrage.
- Test rapide : ouvrir <http://localhost:5123/api/dashboard>, ou utiliser le fichier
  [`ProjectPilotLite.Api/ProjectPilotLite.Api.http`](ProjectPilotLite.Api/ProjectPilotLite.Api.http).

## Lancer le client

L'API doit tourner au préalable.

**WPF (Windows) :**

```bash
dotnet run --project ProjectPilotLite.Wpf
```

**Avalonia (Windows / macOS / Linux) :**

```bash
dotnet run --project ProjectPilotLite.Avalonia
```

### Configurer l'URL de l'API

Chaque client lit l'URL de l'API dans son fichier `appsettings.json`
(`ProjectPilotLite.Wpf/appsettings.json` ou `ProjectPilotLite.Avalonia/appsettings.json`) :

```json
{ "ApiBaseUrl": "http://localhost:5123" }
```

Il suffit de modifier cette valeur pour pointer vers une API distante ; aucune recompilation du
code n'est nécessaire.

## Fonctionnalités réalisées

- **Projets** : création, liste, détail, modification du statut (Prévu / En cours / Terminé / Bloqué).
- **Tâches** : ajout à un projet, liste par projet, changement de statut (À faire / En cours / Terminé),
  priorité (Basse / Normale / Haute).
- **Livrables** : ajout, liste par projet, validation / refus (Déposé / Validé / Refusé),
  type (Code / Documentation / Présentation / Autre), commentaire.
- **Tableau de bord** : nombre de projets, projets en cours, projets bloqués, total des tâches,
  tâches terminées, livrables déposés (agrégé côté API).

### Endpoints principaux

| Méthode | Route | Rôle |
|---|---|---|
| GET | `/api/projects` | Liste des projets |
| GET | `/api/projects/{id}` | Détail (tâches + livrables) |
| POST | `/api/projects` | Créer un projet |
| PATCH | `/api/projects/{id}/status` | Modifier le statut d'un projet |
| GET / POST | `/api/projects/{id}/tasks` | Tâches d'un projet |
| PATCH | `/api/tasks/{id}/status` | Statut d'une tâche |
| GET / POST | `/api/projects/{id}/deliverables` | Livrables d'un projet |
| PATCH | `/api/deliverables/{id}/status` | Valider / refuser un livrable |
| GET | `/api/dashboard` | Synthèse d'avancement |

## Intégration continue

Le pipeline [`.github/workflows/ci.yml`](.github/workflows/ci.yml) se déclenche à chaque `push` et
`pull request` :

- **Job Windows** (`windows-latest`) : `restore` → `build` de la solution complète (WPF inclus) →
  `publish` de l'API.
- **Job Linux** (`ubuntu-latest`) : `restore` → `build` de l'API + Core + Client.Shared + Avalonia →
  `publish` de l'API.

État du pipeline : voir le badge en haut de ce fichier ou l'onglet
[Actions](https://github.com/yonilancry/project-pilot-lite/actions).

## Limites connues

- Le **rendu** du client WPF ne peut être vérifié que sous Windows ; sur macOS/Linux, utiliser le
  client Avalonia.
- L'API est **ouverte** (pas d'authentification) — conforme au périmètre de l'évaluation.
- Avertissement de restauration `NU1903` sur `SQLitePCLRaw.lib.e_sqlite3` (dépendance transitive
  d'EF Core Sqlite) : aucune version corrigée n'est publiée en amont à ce jour. Sans impact sur un
  usage local ; à surveiller pour une future montée de version.
