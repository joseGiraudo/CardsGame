-- drop DATABASE cardsgamedb;

CREATE DATABASE cardsgamedb;
USE cardsgamedb;

-- Tabla de países
CREATE TABLE countries (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(70) NOT NULL
);

-- Tabla de usuarios
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    countryId INT,
    avatar VARCHAR(255),
    role ENUM('Admin', 'Organizer', 'Judge', 'Player') NOT NULL,
    createdBy INT, -- Referencia al usuario que lo creó
    createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (countryId) REFERENCES countries(id) ON DELETE SET NULL,
    FOREIGN KEY (createdBy) REFERENCES users(id) ON DELETE SET NULL
);

INSERT INTO countries (NAME) VALUES('ARG');

INSERT INTO users (name, username, email, password, countryId, avatar, role, createdBy)
 VALUES ('tester', 'tester', 'tester@correo.com', 'tester123', 1, 'https://example.com/avatar.png', 'Player', NULL);
 
 
INSERT INTO users (name, username, email, password, countryId, avatar, role, createdBy)
 VALUES ('Admin', 'admin', 'admin@correo.com', 'admin123', 1, 'https://example.com/avatar.png', 'Admin', NULL);

 -- SELECT * FROM users;
 -- SELECT * FROM countries;
 -- SELECT * FROM tournaments;
 -- SELECT * FROM decks;
 -- SELECT * FROM decks_cards; 
 -- SELECT * FROM series;
 -- SELECT * FROM cards_series; 
 
-- Tabla de cartas
CREATE TABLE cards (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    attack INT NOT NULL,
    defense INT NOT NULL,
    illustration VARCHAR(255)
);

-- Tabla de series
CREATE TABLE series (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    releaseDate DATE NOT NULL
);

-- Relación entre cartas y series
CREATE TABLE cards_series (
    cardId INT NOT NULL,
    seriesId INT NOT NULL,
    PRIMARY KEY (cardId, seriesId),
    FOREIGN KEY (cardId) REFERENCES cards(id) ON DELETE CASCADE,
    FOREIGN KEY (seriesId) REFERENCES series(id) ON DELETE CASCADE
);

-- Colecciones de cartas de los jugadores
CREATE TABLE collections (
    playerId INT NOT NULL,
    cardId INT NOT NULL,
    PRIMARY KEY (playerId, cardId),
    FOREIGN KEY (playerId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (cardId) REFERENCES cards(id) ON DELETE CASCADE
);

-- Mazos de cartas de un jugador
CREATE TABLE decks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    playerId INT NOT NULL,
    name VARCHAR(50),
    FOREIGN KEY (playerId) REFERENCES users(id) ON DELETE CASCADE
);

-- Cartas en los mazos
CREATE TABLE decks_cards (
    deckId INT NOT NULL,
    cardId INT NOT NULL,
    PRIMARY KEY (deckId, cardId),
    FOREIGN KEY (deckId) REFERENCES decks(id) ON DELETE CASCADE,
    FOREIGN KEY (cardId) REFERENCES cards(id) ON DELETE CASCADE
);

-- Torneos
CREATE TABLE tournaments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    startDate DATETIME NOT NULL,
    endDate DATETIME NOT NULL,
    countryId INT,
    phase ENUM('Registration', 'InProgress', 'Finished', 'Canceled') DEFAULT 'Registration',
    organizerId INT NOT NULL,
    winnerId INT, -- Ganador del torneo
    FOREIGN KEY (countryId) REFERENCES countries(id) ON DELETE SET NULL,
    FOREIGN KEY (organizerId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (winnerId) REFERENCES users(id) ON DELETE SET NULL
);

-- Series disponibles en un torneo
CREATE TABLE tournament_series (
    tournamentId INT NOT NULL,
    seriesId INT NOT NULL,
    PRIMARY KEY (tournamentId, seriesId),
    FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE,
    FOREIGN KEY (seriesId) REFERENCES series(id) ON DELETE CASCADE
);

CREATE TABLE tournament_players (
    tournamentId INT,
    playerId INT,
    deckId INT, -- Referencia al mazo del jugador para este torneo
    PRIMARY KEY (tournamentId, playerId),
    FOREIGN KEY (tournamentId) REFERENCES tournaments(Id),
    FOREIGN KEY (playerId) REFERENCES users(Id),
    FOREIGN KEY (deckId) REFERENCES decks(Id)
);

-- Jueces asignados a torneos
CREATE TABLE tournament_judges (
    tournamentId INT NOT NULL,
    judgeId INT NOT NULL,
    PRIMARY KEY (tournamentId, judgeId),
    FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE,
    FOREIGN KEY (judgeId) REFERENCES users(id) ON DELETE CASCADE
);


-- Juegos dentro de torneos
CREATE TABLE games (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tournamentId INT NOT NULL,
    startDate DATETIME, -- puedo permitir null para los aprtidos que no se juegan o doy otr fecha?
    player1 INT NOT NULL,
    player2 INT, -- permito null para los partidos que no se juegan?
    winnerId INT,
    FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE,
    FOREIGN KEY (player1) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (player2) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (winnerId) REFERENCES users(id) ON DELETE SET NULL
);

-- Descalificaciones de jugadores
CREATE TABLE disqualifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    playerId INT NOT NULL,
    tournamentId INT NOT NULL,
    judgeId INT NOT NULL,
    reason VARCHAR(255),
    FOREIGN KEY (playerId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (tournamentId) REFERENCES tournaments(id) ON DELETE CASCADE,
    FOREIGN KEY (judgeId) REFERENCES users(id) ON DELETE CASCADE
);
   
CREATE TABLE refresh_tokens (
	id INT AUTO_INCREMENT PRIMARY KEY,
	userId INT NOT NULL,
	token VARCHAR(255) NOT NULL,
	expiration DATETIME NOT NULL,
   createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE
);

/*

-- Select para obtener los jugadores invistos de un torneo
SELECT DISTINCT u.id, u.name
FROM users u
JOIN games g ON u.id = g.player1 OR u.id = g.player2
WHERE g.tournamentId = @tournamentId
AND u.id NOT IN (
    SELECT DISTINCT CASE 
        WHEN g.player1 != g.winnerId THEN g.player1
        WHEN g.player2 != g.winnerId THEN g.player2
    END
    FROM games g
    WHERE g.tournamentId = @tournamentId
    AND g.winnerId IS NOT NULL
);

-- Consulta para saber si todas las cartas de un mazo pertenecen
-- a un conjunto de series
SELECT COUNT(*) = (
    SELECT COUNT(*) 
    FROM decks_cards 
    WHERE deckId = @deckId
) AS all_cards_in_series
FROM decks_cards dc
JOIN cards_series cs ON dc.cardId = cs.cardId
WHERE dc.deckId = @deckId AND cs.seriesId IN (@seriesId1, @seriesId2, ...);
	
*/
	
