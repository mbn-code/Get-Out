document.addEventListener('DOMContentLoaded', () => {
    const canvas = document.getElementById('mapCanvas');
    const ctx = canvas.getContext('2d');
    const stageSelect = document.getElementById('stageSelect');
    const roomSelect = document.getElementById('roomSelect');
    
    // Map image cache
    const mapImages = {};
    
    // Map configurations for each room
    const mapConfig = {
        'train_station_-_open_room': {
            mapUrl: 'maps/train_station_open.png', // Use the relative path to your PNG file
            scale: 1.0,
            defaultPosition: { x: 0, y: 0 }
        },
        'train_station_-_booth': {
            mapUrl: 'maps/train_station_booth.png',
            scale: 1.0,
            defaultPosition: { x: 0, y: 0 }
        },
        'basement': {
            mapUrl: 'maps/basement.png',
            scale: 1.0,
            defaultPosition: { x: 0, y: 0 }
        },
        'police_locker_room': {
            mapUrl: 'maps/police_locker.png',
            scale: 1.0,
            defaultPosition: { x: 0, y: 0 }
        },
        'tile_room': {
            mapUrl: 'maps/tile_room.png',
            scale: 1.0,
            defaultPosition: { x: 0, y: 0 }
        },
        'maze_room': {
            mapUrl: 'maps/maze_room.png',
            scale: 1.0,
            defaultPosition: { x: 0, y: 0 }
        }
    };

    // Load map image
    function loadMapImage(roomId) {
        return new Promise((resolve, reject) => {
            if (mapImages[roomId]) {
                resolve(mapImages[roomId]);
                return;
            }

            const config = mapConfig[roomId];
            if (!config) {
                reject('No map configuration found for this room');
                return;
            }

            // For development: Create a placeholder map
            const tempCanvas = document.createElement('canvas');
            const tempCtx = tempCanvas.getContext('2d');
            tempCanvas.width = 800;
            tempCanvas.height = 600;
              
            // Draw placeholder map
            tempCtx.fillStyle = 'rgba(76, 175, 80, 0.1)';
            tempCtx.fillRect(0, 0, tempCanvas.width, tempCanvas.height);
            tempCtx.strokeStyle = 'rgba(76, 175, 80, 0.5)';
            tempCtx.lineWidth = 2;
            tempCtx.strokeRect(10, 10, tempCanvas.width - 20, tempCanvas.height - 20);
            
            // Add room name
            tempCtx.fillStyle = 'rgba(76, 175, 80, 0.5)';
            tempCtx.font = '24px Arial';
            tempCtx.textAlign = 'center';
            tempCtx.fillText(roomId.replace(/_/g, ' ').toUpperCase(), tempCanvas.width/2, tempCanvas.height/2);
            
            // Create grid pattern
            tempCtx.beginPath();
            for(let x = 50; x < tempCanvas.width; x += 50) {
                tempCtx.moveTo(x, 0);
                tempCtx.lineTo(x, tempCanvas.height);
            }
            for(let y = 50; y < tempCanvas.height; y += 50) {
                tempCtx.moveTo(0, y);
                tempCtx.lineTo(tempCanvas.width, y);
            }
            tempCtx.strokeStyle = 'rgba(76, 175, 80, 0.2)';
            tempCtx.lineWidth = 1;
            tempCtx.stroke();

            // Store the placeholder
            const img = new Image();
            img.src = tempCanvas.toDataURL();
            img.onload = () => {
                mapImages[roomId] = img;
                resolve(img);
            };
            
            // When you're ready to use real images, uncomment this:
            /*
            const img = new Image();
            img.onload = () => {
                mapImages[roomId] = img;
                resolve(img);
            };
            img.onerror = () => reject('Failed to load map image');
            img.src = config.mapUrl;
            */
        });
    }

    // Draw map on canvas
    function drawMap(roomId) {
        loadMapImage(roomId)
            .then(img => {
                // Clear the canvas
                ctx.clearRect(0, 0, canvas.width, canvas.height);
                
                // Calculate scaling to fit the canvas while maintaining aspect ratio
                const scale = Math.min(
                    canvas.width / img.width,
                    canvas.height / img.height
                );
                
                // Center the image
                const x = (canvas.width - img.width * scale) / 2;
                const y = (canvas.height - img.height * scale) / 2;
                
                // Draw the image
                ctx.drawImage(img, x, y, img.width * scale, img.height * scale);
            })
            .catch(error => {
                console.error('Error loading map:', error);
                // Display an error message on the canvas
                ctx.fillStyle = 'rgba(255, 0, 0, 0.5)';
                ctx.font = '16px Arial';
                ctx.textAlign = 'center';
                ctx.fillText('Failed to load map image.', canvas.width / 2, canvas.height / 2);
            });
    }

    // Load map image
    function loadMapImage(roomId) {
        return new Promise((resolve, reject) => {
            if (mapImages[roomId]) {
                resolve(mapImages[roomId]);
                return;
            }

            const config = mapConfig[roomId];
            if (!config) {
                reject(`No map configuration found for room ID: ${roomId}`);
                return;
            }

            const img = new Image();
            img.onload = () => {
                mapImages[roomId] = img; // Cache the image
                resolve(img);
            };
            img.onerror = () => reject(`Failed to load map image: ${config.mapUrl}`);
            img.src = config.mapUrl; // Use the specified path in the mapConfig
        });
    }

    // Update room options based on stage
    function updateRoomOptions(stage) {
        roomSelect.innerHTML = '<option value="">Select Room...</option>';
        if (!stage) return;

        const rooms = {
            'stage1': ['Train Station - Open Room', 'Train Station - Booth'],
            'stage2': ['Basement'],
            'stage3': ['Police Locker Room'],
            'stage4': ['Tile Room', 'Maze Room']
        };

        rooms[stage].forEach(room => {
            const option = document.createElement('option');
            option.value = room.toLowerCase().replace(/\s+/g, '_');
            option.textContent = room;
            roomSelect.appendChild(option);
        });
    }

    function updateRoomInfo(stage, room) {
        const infoPanel = document.getElementById('roomInfo');
        if (!stage || !room) {
            infoPanel.textContent = 'Select a stage and room to view information';
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            return;
        }
        
        // Room-specific information and symbols
        const roomInfo = {
            'train_station_-_open_room': {
                description: 'Location: Main area of the train station\nSymbols: 🎫 Ticket, 🧳 Luggage, 📰 Newspaper, 🪑 Bench',
                symbols: ['🎫', '🧳', '📰', '🪑']
            },
            'train_station_-_booth': {
                description: 'Location: Ticket booth area\nSymbols: 💰 Cash Register, 📢 Intercom, 🎟️ Tickets, 🖨️ Printer',
                symbols: ['📠']
            },
            'basement': {
                description: 'Location: Below ground level\nSymbols: 🔧 Wrench, 📰 Newspaper, 🛢️ Barrels, 🧰 Toolbox',
                symbols: ['🔧', '📰', '🛢️', '🧰']
            },
            'police_locker_room': {
                description: 'Location: Police station wing\nSymbols: ⭐ Police Badge, 🧥 Coat, 📋 Documents , 🗄️ Locker',
                symbols: ['⭐', '🧥', '📋', '🗄️']
            },
            'tile_room': {
                description: 'Location: Maintenance section\nSymbols: ⬜ Tilefloor, 🟥 Notice board, 🟨 Notice board, 🟩 Notice board',
                symbols: ['⬜', '🟥', '🟨', '🟩']
            },
            'maze_room': {
                description: 'Location: Maze room\nSymbols: 🗝️ Key',
                symbols: ['🗝️']
            }
        };

        const info = roomInfo[room];
        if (info) {
            infoPanel.innerHTML = `<pre>== ROOM INFORMATION ==\n${info.description}</pre>`;
            updateSymbolGrid(info.symbols);
            drawMap(room);
        }
    }

    function updateSymbolGrid(symbols) {
        const symbolGrid = document.querySelector('.symbol-grid');
        symbolGrid.innerHTML = '';
    
        // Check if symbols exist and is a non-empty array
        if (symbols && Array.isArray(symbols) && symbols.length > 0) {
            symbols.forEach(symbol => {
                const button = document.createElement('button');
                button.className = 'symbol-btn';
                button.setAttribute('data-symbol', symbol);
                button.textContent = symbol;
                symbolGrid.appendChild(button);
            });
        } else {
            // Handle case where no symbols are provided
            const noSymbolMessage = document.createElement('p');
            noSymbolMessage.textContent = 'No symbols available for this room.';
            symbolGrid.appendChild(noSymbolMessage);
        }
    }
    

    // Clue discovery system
    const clues = {
        'train_station_-_open_room': {
            description: 'A suspicious ticket found on the ground.',
            discovered: false
        },
        'train_station_-_booth': {
            description: 'A hidden compartment in the booth.',
            discovered: false
        },
        // Add more clues for other rooms...
    };

    // Function to handle clue discovery
    function discoverClue(roomId) {
        const clue = clues[roomId];
        if (clue && !clue.discovered) {
            clue.discovered = true;
            showClueModal(clue.description);
        }
    }

    // Show clue modal
    function showClueModal(description) {
        const modal = document.getElementById('clueModal');
        const clueDescription = document.getElementById('clueDescription');
        clueDescription.textContent = description;
        modal.style.display = 'block';
    }

    // Close modal functionality
    document.querySelector('.close').onclick = function() {
        document.getElementById('clueModal').style.display = 'none';
    };

    // Add to evidence log functionality
    document.getElementById('addToEvidenceLog').onclick = function() {
        // Logic to add clue to evidence log
        console.log('Clue added to evidence log');
        document.getElementById('clueModal').style.display = 'none';
    };

    // Event listener for tool buttons
    document.querySelectorAll('.tool-btn').forEach(button => {
        button.addEventListener('click', (e) => {
            const tool = e.target.dataset.tool;
            // Placeholder for tool interactions
            console.log(`Tool used: ${tool}`);
        });
    });

    // Event listener for Download Game button
    document.getElementById('downloadGameBtn').addEventListener('click', () => {
        window.location.href = 'game.zip'; // Update with the correct path to your zip file
    });

    // Event Listeners
    stageSelect.addEventListener('change', (e) => {
        const stage = e.target.value;
        updateRoomOptions(stage);
        updateRoomInfo(stage, roomSelect.value);
    });

    roomSelect.addEventListener('change', (e) => {
        const room = e.target.value;
        const stage = stageSelect.value;
        updateRoomInfo(stage, room);
    });

    // Initialize the canvas
    function resizeCanvas() {
        const container = canvas.parentElement;
        canvas.width = container.clientWidth - 40;
        canvas.height = container.clientHeight - 40;
        
        // Redraw current map if any
        if (roomSelect.value) {
            drawMap(roomSelect.value);
        }
    }
    
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);
});
