# BeFit - Aplikacja do zarządzania treningami

Aplikacja ASP.NET Core MVC z Identity do zarządzania sesjami treningowymi, typami ćwiczeń i śledzenia postępów.

## Wymagania

- .NET 8.0 SDK lub nowszy
- PostgreSQL 12 lub nowszy
- Git (opcjonalnie, do klonowania repozytorium)

## Instalacja i konfiguracja

### Krok 1: Klonowanie repozytorium

```bash
git clone <url-repozytorium>
cd ProjectASP_befit
```

### Krok 2: Instalacja PostgreSQL

#### macOS (używając Homebrew)

```bash
brew install postgresql@14
brew services start postgresql@14
```

#### Windows

1. Pobierz instalator PostgreSQL z [oficjalnej strony](https://www.postgresql.org/download/windows/)
2. Zainstaluj PostgreSQL, zapamiętując hasło użytkownika `postgres`
3. Upewnij się, że usługa PostgreSQL jest uruchomiona

### Krok 3: Utworzenie bazy danych

#### macOS/Linux

```bash
# Zaloguj się do PostgreSQL
psql -U postgres

# W konsoli PostgreSQL wykonaj:
CREATE DATABASE befit;
\q
```

#### Windows

1. Otwórz "SQL Shell (psql)" lub "pgAdmin"
2. Zaloguj się używając użytkownika `postgres`
3. Wykonaj komendę:
```sql
CREATE DATABASE befit;
```

### Krok 4: Konfiguracja connection string

Edytuj plik `appsettings.json` lub `appsettings.Development.json` i zaktualizuj connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=befit;Username=postgres;Password=TWÓJ_PASSWORD"
  }
}
```

**UWAGA:** Zamień `TWÓJ_PASSWORD` na rzeczywiste hasło użytkownika PostgreSQL.

### Krok 5: Przywracanie pakietów NuGet

```bash
dotnet restore
```

### Krok 6: Utworzenie migracji bazy danych

```bash
dotnet ef migrations add InitialCreate
```

### Krok 7: Zastosowanie migracji

```bash
dotnet ef database update
```

### Krok 8: Uruchomienie aplikacji

```bash
dotnet run
```

Aplikacja będzie dostępna pod adresem: `https://localhost:5001` lub `http://localhost:5000`

## Pierwsze kroki

### 1. Rejestracja użytkownika

1. Przejdź do strony rejestracji (link w prawym górnym rogu)
2. Utwórz konto użytkownika

### 2. Konto administratora

**Konto administratora jest tworzone automatycznie przy pierwszym uruchomieniu aplikacji.**

**Dane logowania:**
- **Email:** `admin@befit.com`
- **Hasło:** `Admin123!`

**UWAGA:** Ze względów bezpieczeństwa zmień hasło administratora po pierwszym logowaniu!

Jeśli chcesz utworzyć dodatkowe konto administratora ręcznie, możesz użyć konsoli PostgreSQL:

```sql
-- Znajdź ID użytkownika
SELECT Id, UserName FROM "AspNetUsers";

-- Dodaj użytkownika do roli Administrator (zamień 'USER_ID' na rzeczywiste ID)
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT 'USER_ID', "Id" FROM "AspNetRoles" WHERE "Name" = 'Administrator';
```

### 3. Dodanie typów ćwiczeń (tylko dla administratorów)

1. Zaloguj się jako administrator
2. Przejdź do "Typy ćwiczeń" w menu
3. Kliknij "Dodaj nowy typ ćwiczenia" (dostępne tylko dla administratorów)
4. Wypełnij formularz i zapisz

### 4. Dodanie sesji treningowej

1. Zaloguj się jako użytkownik
2. Przejdź do "Sesje treningowe"
3. Kliknij "Dodaj nową sesję treningową"
4. Wypełnij datę i czas rozpoczęcia oraz zakończenia
5. Opcjonalnie dodaj uwagi
6. Zapisz

### 5. Dodanie wykonanych ćwiczeń

1. Przejdź do "Wykonane ćwiczenia"
2. Kliknij "Dodaj nowe wykonane ćwiczenie"
3. Wybierz sesję treningową i typ ćwiczenia
4. Wypełnij obciążenie, liczbę serii i powtórzeń
5. Zapisz

### 6. Przeglądanie statystyk

1. Przejdź do "Statystyki" w menu
2. Zobacz statystyki z ostatnich 4 tygodni:
   - Liczba wykonanych ćwiczeń każdego typu
   - Łączna liczba powtórzeń
   - Średnie i maksymalne obciążenie

## Struktura projektu

```
BeFit/
├── Controllers/          # Kontrolery MVC
│   ├── ExerciseTypesController.cs      # Zarządzanie typami ćwiczeń (admin)
│   ├── TrainingSessionsController.cs    # Zarządzanie sesjami treningowymi
│   ├── ExerciseExecutionsController.cs # Zarządzanie wykonanymi ćwiczeniami
│   └── StatisticsController.cs          # Statystyki użytkownika
├── Models/               # Modele danych
│   ├── ExerciseType.cs                 # Typ ćwiczenia
│   ├── TrainingSession.cs              # Sesja treningowa
│   └── ExerciseExecution.cs            # Wykonane ćwiczenie
├── Views/                # Widoki Razor
│   ├── ExerciseTypes/    # Widoki typów ćwiczeń
│   ├── TrainingSessions/ # Widoki sesji treningowych
│   ├── ExerciseExecutions/ # Widoki wykonanych ćwiczeń
│   └── Statistics/       # Widok statystyk
├── Data/                 # Kontekst bazy danych
│   └── ApplicationDbContext.cs
└── Program.cs            # Konfiguracja aplikacji
```

## Funkcjonalności

### Typy ćwiczeń
- **Wyświetlanie:** Dostępne dla wszystkich (nawet niezalogowanych)
- **Tworzenie/Edytowanie/Usuwanie:** Tylko dla administratorów

### Sesje treningowe
- **Tworzenie:** Każdy zarejestrowany użytkownik
- **Wyświetlanie/Edytowanie/Usuwanie:** Tylko własne sesje użytkownika
- Automatyczne przypisanie do konta użytkownika

### Wykonane ćwiczenia
- **Tworzenie:** Każdy zarejestrowany użytkownik
- **Wyświetlanie/Edytowanie/Usuwanie:** Tylko własne ćwiczenia użytkownika
- Automatyczne przypisanie do konta użytkownika przez sesję treningową

### Statystyki
- Statystyki z ostatnich 4 tygodni
- Dla każdego typu ćwiczenia:
  - Liczba wykonanych ćwiczeń
  - Łączna liczba powtórzeń (serie × powtórzenia)
  - Średnie obciążenie
  - Maksymalne obciążenie

## Walidacja

Wszystkie modele mają ustawioną walidację:

- **ExerciseType:**
  - Nazwa: wymagana, 2-100 znaków
  - Opis: opcjonalny, max 500 znaków

- **TrainingSession:**
  - Data rozpoczęcia: wymagana
  - Data zakończenia: wymagana, musi być późniejsza niż data rozpoczęcia
  - Uwagi: opcjonalne, max 1000 znaków

- **ExerciseExecution:**
  - Obciążenie: wymagane, 0.01-1000 kg
  - Liczba serii: wymagana, 1-100
  - Liczba powtórzeń: wymagana, 1-1000
  - Uwagi: opcjonalne, max 500 znaków

## Bezpieczeństwo

- Użytkownicy mogą edytować/usunąć tylko swoje własne sesje treningowe i wykonane ćwiczenia
- Typy ćwiczeń mogą być zarządzane tylko przez administratorów
- Wszystkie formularze mają walidację po stronie klienta i serwera
- Zabezpieczenia przed atakami CSRF

## Rozwiązywanie problemów

### Problem: Nie można połączyć się z bazą danych

**Rozwiązanie:**
1. Sprawdź, czy PostgreSQL jest uruchomiony:
   - macOS: `brew services list`
   - Windows: Sprawdź w "Services" (services.msc)
2. Sprawdź connection string w `appsettings.json`
3. Sprawdź, czy baza danych `befit` istnieje

### Problem: Błędy migracji

**Rozwiązanie:**
```bash
# Usuń stare migracje (jeśli istnieją)
dotnet ef migrations remove

# Utwórz nowe migracje
dotnet ef migrations add InitialCreate

# Zastosuj migracje
dotnet ef database update
```

### Problem: Aplikacja nie uruchamia się na Windows

**Rozwiązanie:**
1. Upewnij się, że masz zainstalowany .NET 8.0 SDK
2. Sprawdź, czy PostgreSQL jest uruchomiony
3. Sprawdź connection string w `appsettings.json` (użyj backslashes dla ścieżek Windows jeśli potrzeba)

## Technologie

- ASP.NET Core 8.0 MVC
- Entity Framework Core 8.0
- PostgreSQL (Npgsql)
- ASP.NET Core Identity
- Bootstrap 5
- jQuery

## Licencja

Ten projekt został stworzony w celach edukacyjnych.

## Autor

Dawid 148985

