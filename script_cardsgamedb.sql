CREATE DATABASE cardsgamedb;

USE cardsgamedb;

-- Tabla de países
CREATE TABLE countries (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(70) NOT NULL
);

INSERT INTO countries (NAME) VALUES('ARG')

 INSERT INTO users (name, username, email, password, countryId, avatar, role, createdBy)
 VALUES ('tester', 'tester', 'tester@correo.com', 'tester123', 1, 'https://example.com/avatar.png', 'Jugador', NULL);

 SELECT * FROM users


-- Tabla de usuarios
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    countryId INT,
    avatar VARCHAR(255),
    role ENUM('Administrador', 'Organizador', 'Juez', 'Jugador') NOT NULL,
    createdBy INT, -- Referencia al usuario que lo creó
    createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (countryId) REFERENCES countries(id) ON DELETE SET NULL,
    FOREIGN KEY (createdBy) REFERENCES users(id) ON DELETE SET NULL
);

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
    tournamentId INT NOT NULL,
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
    endDate DATETIME,
    country VARCHAR(50), -- cambiar a countryId y referenciarlo
    phase ENUM('Registro', 'Torneo', 'Finalizado') DEFAULT 'Registro',
    organizerId INT NOT NULL,
    winnerId INT, -- Ganador del torneo
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
    startDate DATETIME NOT NULL,
    player1 INT NOT NULL,
    player2 INT NOT NULL,
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
    judgeId INT NOT NULL,
    reason VARCHAR(255),
    FOREIGN KEY (playerId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (judgeId) REFERENCES users(id) ON DELETE CASCADE
);
   
