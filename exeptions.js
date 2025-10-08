// exceptions.js
// Custom exceptions for your project with timestamp and optional duration logging

class CustomError extends Error {
    constructor(message, statusCode = 500) {
        super(message);
        this.name = this.constructor.name;
        this.statusCode = statusCode;
        this.timestamp = new Date(); // When the error occurred
    }

    // Optionally log this error
    log() {
        console.error(`[${this.timestamp.toISOString()}] ${this.name} (${this.statusCode}): ${this.message}`);
    }
}

class NotFoundError extends CustomError {
    constructor(message = "Resource not found") {
        super(message, 404);
    }
}

class ValidationError extends CustomError {
    constructor(message = "Invalid data") {
        super(message, 400);
    }
}

// Utility function to measure duration of a function
async function measureDuration(fn, ...args) {
    const start = Date.now();
    try {
        const result = await fn(...args);
        return result;
    } finally {
        const end = Date.now();
        console.log(`Function ${fn.name} executed in ${end - start}ms`);
    }
}

module.exports = {
    CustomError,
    NotFoundError,
    ValidationError,
    measureDuration
};
