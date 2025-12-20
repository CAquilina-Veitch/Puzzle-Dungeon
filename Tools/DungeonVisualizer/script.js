/**
 * Dungeon Visualizer Core Logic
 */

// --- Constants & Config ---
const TILE_SIZE = 40;
const MINI_TILE_SIZE = 20;

// --- State Management ---
const State = {
    mode: 'edit', // 'edit' | 'play'
    tool: 'shape', // 'shape' | 'door'
    rooms: [], // List of placed room instances
    roomDefinitions: [], // List of saved room definitions
    savedLayout: null, // Snapshot of rooms for reset
    playerPos: { x: 0, y: 0 },
    savedPlayerPos: { x: 0, y: 0 },
    playerStart: null, // { x, y } for player's starting position
    settingStart: false, // Flag to indicate if user is setting start position
    editModeLayout: null, // Snapshot of layout when entering play mode
    isDragging: false,
    dragOffset: { x: 0, y: 0 },
    activeRoomId: null,
    nextRoomId: 1,
    dungeonSize: { w: 10, h: 10 }
};

// --- DOM Elements ---
const elements = {
    miniGrid: document.getElementById('mini-grid'),
    mainGrid: document.getElementById('main-grid'),
    roomPalette: document.getElementById('room-palette'),
    playerToken: document.getElementById('player-token'),
    statusText: document.getElementById('status-text'),
    coordsText: document.getElementById('coordinates-text'),
    btnSaveRoom: document.getElementById('btn-save-room'),
    btnSaveLayout: document.getElementById('btn-save-layout'),
    btnResetLayout: document.getElementById('btn-reset-layout'),
    btnClearAll: document.getElementById('btn-clear-all'),
    btnSetStart: document.getElementById('btn-set-start'),
    modeEdit: document.getElementById('mode-edit'),
    modePlay: document.getElementById('mode-play'),
    toolShape: document.getElementById('tool-shape'),
    toolDoor: document.getElementById('tool-door'),
    inputWidth: document.getElementById('dungeon-width'),
    inputHeight: document.getElementById('dungeon-height')
};

// --- Helper Classes ---

class RoomDefinition {
    constructor(name, tiles, doors, color) {
        this.name = name;
        this.tiles = tiles; // Array of {x, y} relative to center
        this.doors = doors; // Array of {x, y, dir} (dir: 'top', 'right', 'bottom', 'left')
        this.color = color || '#3b82f6'; // Default blue if not provided
    }
}

class PlacedRoom {
    constructor(id, definition, x, y) {
        this.id = id;
        this.definition = definition;
        this.x = x; // Grid X
        this.y = y; // Grid Y
        this.element = null; // DOM Element
    }
}

// --- Systems ---

const RoomCreator = {
    gridSize: 7,
    activeTiles: new Set(), // "x,y" strings
    doors: [], // {x, y, dir} objects

    init() {
        this.renderGrid();
        elements.miniGrid.addEventListener('mousedown', (e) => this.handleInput(e));
        elements.btnSaveRoom.addEventListener('click', () => this.saveRoom());

        elements.toolShape.addEventListener('click', () => this.setTool('shape'));
        elements.toolDoor.addEventListener('click', () => this.setTool('door'));

        // Initial name
        document.getElementById('room-name').value = `Room ${State.nextRoomId}`;
    },

    setTool(tool) {
        State.tool = tool;
        elements.toolShape.classList.toggle('active', tool === 'shape');
        elements.toolDoor.classList.toggle('active', tool === 'door');
    },

    loadRoom(def) {
        this.activeTiles.clear();
        this.doors = [];

        // Load tiles
        def.tiles.forEach(t => {
            this.activeTiles.add(`${t.x},${t.y}`);
        });

        // Load doors (deep copy)
        this.doors = def.doors.map(d => ({ ...d }));

        // Set name
        document.getElementById('room-name').value = def.name;

        this.renderGrid();
    },

    renderGrid() {
        elements.miniGrid.innerHTML = '';
        const offset = Math.floor(this.gridSize / 2);

        for (let y = -offset; y <= offset; y++) {
            for (let x = -offset; x <= offset; x++) {
                const cell = document.createElement('div');
                cell.className = 'mini-cell';
                cell.dataset.x = x;
                cell.dataset.y = y;

                const key = `${x},${y}`;
                if (this.activeTiles.has(key)) {
                    cell.classList.add('active');

                    // Render doors
                    this.doors.filter(d => d.x === x && d.y === y).forEach(d => {
                        const doorEl = document.createElement('div');
                        doorEl.className = `door-marker door-${d.dir}`;
                        cell.appendChild(doorEl);
                    });
                }

                elements.miniGrid.appendChild(cell);
            }
        }
    },

    handleInput(e) {
        if (!e.target.classList.contains('mini-cell') && !e.target.classList.contains('door-marker')) return;

        // Get cell coordinates
        const cell = e.target.closest('.mini-cell');
        const x = parseInt(cell.dataset.x);
        const y = parseInt(cell.dataset.y);
        const key = `${x},${y}`;

        if (State.tool === 'shape') {
            if (this.activeTiles.has(key)) {
                this.activeTiles.delete(key);
                // Remove associated doors
                this.doors = this.doors.filter(d => !(d.x === x && d.y === y));
            } else {
                this.activeTiles.add(key);
            }
        } else if (State.tool === 'door') {
            if (!this.activeTiles.has(key)) return; // Can only add doors to active tiles

            // Determine clicked edge based on mouse position within cell
            const rect = cell.getBoundingClientRect();
            const relX = e.clientX - rect.left;
            const relY = e.clientY - rect.top;
            const w = rect.width;
            const h = rect.height;

            // Simple diagonal checks for quadrants
            // Top-Left to Bottom-Right: y = x
            // Bottom-Left to Top-Right: y = h - x

            let dir = '';
            if (relY < relX && relY < h - relX) dir = 'top';
            else if (relY < relX && relY > h - relX) dir = 'right';
            else if (relY > relX && relY > h - relX) dir = 'bottom';
            else if (relY > relX && relY < h - relX) dir = 'left';

            // Toggle door
            const existingIdx = this.doors.findIndex(d => d.x === x && d.y === y && d.dir === dir);
            if (existingIdx >= 0) {
                this.doors.splice(existingIdx, 1);
            } else {
                this.doors.push({ x, y, dir });
            }
        }

        this.renderGrid();
    },

    saveRoom() {
        if (this.activeTiles.size === 0) return;

        const tiles = Array.from(this.activeTiles).map(k => {
            const [x, y] = k.split(',').map(Number);
            return { x, y };
        });

        const nameInput = document.getElementById('room-name');
        const name = nameInput.value || `Room ${State.nextRoomId}`;

        // Generate Random Color (Pastel/Vibrant)
        const hue = Math.floor(Math.random() * 360);
        const color = `hsl(${hue}, 70%, 40%)`;

        const def = new RoomDefinition(name, tiles, [...this.doors], color);
        LayoutManager.addToPalette(def);

        // Reset creator
        this.activeTiles.clear();
        this.doors = [];
        this.renderGrid();

        // Update next ID and reset name input
        State.nextRoomId++;
        nameInput.value = `Room ${State.nextRoomId}`;
    }
};

const LayoutManager = {
    init() {
        elements.btnSaveLayout.addEventListener('click', () => this.saveLayout());
        elements.btnResetLayout.addEventListener('click', () => this.resetLayout());
        elements.btnClearAll.addEventListener('click', () => this.clearAll());

        // Set Start Button
        elements.btnSetStart.addEventListener('click', () => {
            State.settingStart = !State.settingStart;
            elements.btnSetStart.classList.toggle('active', State.settingStart);
            elements.statusText.textContent = State.settingStart ? "Click grid to set Start Position" : "Ready";
        });

        elements.inputWidth.addEventListener('change', (e) => this.updateDungeonSize());
        elements.inputHeight.addEventListener('change', (e) => this.updateDungeonSize());

        // Global drag handlers
        document.addEventListener('mousemove', (e) => this.onDragMove(e));
        document.addEventListener('mouseup', (e) => this.onDragEnd(e));

        // Grid Click for Start Pos
        elements.mainGrid.addEventListener('click', (e) => this.onGridClick(e));

        this.updateDungeonSize();
        this.renderStartMarker();
    },

    updateDungeonSize() {
        const w = parseInt(elements.inputWidth.value);
        const h = parseInt(elements.inputHeight.value);
        State.dungeonSize = { w, h };

        elements.mainGrid.style.width = `${w * TILE_SIZE}px`;
        elements.mainGrid.style.height = `${h * TILE_SIZE}px`;
    },

    onGridClick(e) {
        if (!State.settingStart) return;

        const rect = elements.mainGrid.getBoundingClientRect();
        const gridX = Math.floor((e.clientX - rect.left) / TILE_SIZE);
        const gridY = Math.floor((e.clientY - rect.top) / TILE_SIZE);

        if (gridX >= 0 && gridX < State.dungeonSize.w && gridY >= 0 && gridY < State.dungeonSize.h) {
            State.playerStart = { x: gridX, y: gridY };
            State.playerPos = { ...State.playerStart }; // Move player immediately to visualize
            this.renderStartMarker();
            PlayerController.updateVisuals();

            // Turn off mode
            State.settingStart = false;
            elements.btnSetStart.classList.remove('active');
            elements.statusText.textContent = "Start Position Set";
        }
    },

    renderStartMarker() {
        let marker = document.getElementById('start-marker');
        if (!marker) {
            marker = document.createElement('div');
            marker.id = 'start-marker';
            marker.className = 'start-marker';
            marker.textContent = 'S';
            elements.mainGrid.appendChild(marker);
        }

        if (State.playerStart) {
            marker.style.display = 'flex';
            marker.style.transform = `translate(${State.playerStart.x * TILE_SIZE}px, ${State.playerStart.y * TILE_SIZE}px)`;
        } else {
            marker.style.display = 'none';
        }
    },

    addToPalette(def) {
        // Check for overwrite
        const existingIdx = State.roomDefinitions.findIndex(r => r.name === def.name);
        if (existingIdx >= 0) {
            // Overwrite
            State.roomDefinitions[existingIdx] = def;
            this.refreshPalette();
        } else {
            // Add new
            State.roomDefinitions.push(def);
            this.createPaletteItem(def);
        }
    },

    refreshPalette() {
        elements.roomPalette.innerHTML = '';
        State.roomDefinitions.forEach(def => this.createPaletteItem(def));
    },

    createPaletteItem(def) {
        const el = document.createElement('div');
        el.className = 'palette-item';
        el.draggable = true;

        // Preview Canvas
        const previewContainer = document.createElement('div');
        previewContainer.className = 'palette-preview';
        const canvas = document.createElement('canvas');
        canvas.width = 40;
        canvas.height = 40;
        this.renderPreview(canvas, def);
        previewContainer.appendChild(canvas);

        // Info
        const info = document.createElement('div');
        info.className = 'palette-info';

        const nameEl = document.createElement('div');
        nameEl.className = 'palette-name';
        nameEl.textContent = def.name;

        const sizeEl = document.createElement('div');
        sizeEl.className = 'palette-size';
        sizeEl.textContent = `${def.tiles.length} tiles`;

        info.appendChild(nameEl);
        info.appendChild(sizeEl);

        // Delete Button
        const delBtn = document.createElement('button');
        delBtn.className = 'palette-delete';
        delBtn.innerHTML = '&times;';
        delBtn.title = "Delete Room";
        delBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            this.deleteRoom(def.name);
        });

        el.appendChild(previewContainer);
        el.appendChild(info);
        el.appendChild(delBtn);

        // Drag Events
        el.addEventListener('dragstart', (e) => {
            if (State.mode !== 'edit') {
                e.preventDefault();
                return;
            }
            // Store the definition in dataTransfer
            e.dataTransfer.setData('application/json', JSON.stringify(def));
            e.dataTransfer.effectAllowed = 'copy';
        });

        // Click to Edit
        el.addEventListener('click', () => {
            if (State.mode === 'edit') {
                RoomCreator.loadRoom(def);
            }
        });

        elements.roomPalette.appendChild(el);
    },

    deleteRoom(name) {
        State.roomDefinitions = State.roomDefinitions.filter(r => r.name !== name);
        this.refreshPalette();
    },

    renderPreview(canvas, roomDef) {
        const ctx = canvas.getContext('2d');
        const size = 40;
        const gridSize = 5; // 8x8 grid in 40px
        const center = size / 2;

        ctx.fillStyle = roomDef.color || '#3b82f6'; // Use room color

        roomDef.tiles.forEach(t => {
            const x = center + (t.x * gridSize) - (gridSize / 2);
            const y = center + (t.y * gridSize) - (gridSize / 2);
            ctx.fillRect(x, y, gridSize - 1, gridSize - 1);
        });

        // Optional: Draw doors
        ctx.fillStyle = '#fbbf24';
        roomDef.doors.forEach(d => {
            const x = center + (d.x * gridSize) - (gridSize / 2);
            const y = center + (d.y * gridSize) - (gridSize / 2);

            if (d.dir === 'top') ctx.fillRect(x, y - 1, gridSize, 2);
            if (d.dir === 'bottom') ctx.fillRect(x, y + gridSize - 1, gridSize, 2);
            if (d.dir === 'left') ctx.fillRect(x - 1, y, 2, gridSize);
            if (d.dir === 'right') ctx.fillRect(x + gridSize - 1, y, 2, gridSize);
        });
    },

    // Handle dropping from palette to grid
    handleDrop(e) {
        e.preventDefault();
        if (State.mode !== 'edit') return;

        try {
            const data = e.dataTransfer.getData('application/json');
            if (!data) return;

            const def = JSON.parse(data);

            // Calculate grid position
            const rect = elements.mainGrid.getBoundingClientRect();
            const gridX = Math.floor((e.clientX - rect.left) / TILE_SIZE);
            const gridY = Math.floor((e.clientY - rect.top) / TILE_SIZE);

            this.placeRoom(def, gridX, gridY);

        } catch (err) {
            console.error("Drop failed", err);
        }
    },

    placeRoom(def, x, y) {
        const room = new PlacedRoom(State.nextRoomId++, def, x, y);

        // Create DOM
        const el = document.createElement('div');
        el.className = 'dungeon-room';
        el.dataset.id = room.id;

        // Create tiles
        def.tiles.forEach(t => {
            const tile = document.createElement('div');
            tile.className = 'room-tile active';
            tile.style.left = `${t.x * TILE_SIZE}px`;
            tile.style.top = `${t.y * TILE_SIZE}px`;
            tile.style.backgroundColor = def.color || '#3b82f6'; // Apply color
            tile.style.borderColor = adjustColor(def.color || '#3b82f6', -20); // Darker border

            // Add doors
            def.doors.filter(d => d.x === t.x && d.y === t.y).forEach(d => {
                const door = document.createElement('div');
                door.className = `door-marker door-${d.dir}`;
                tile.appendChild(door);
            });

            el.appendChild(tile);
        });

        // Position element
        this.updateElementPosition(el, x, y);

        // Events
        el.addEventListener('mousedown', (e) => this.onRoomMouseDown(e, room));

        elements.mainGrid.appendChild(el);
        room.element = el;
        State.rooms.push(room);
    },

    updateElementPosition(el, x, y) {
        el.style.transform = `translate(${x * TILE_SIZE}px, ${y * TILE_SIZE}px)`;
    },

    onRoomMouseDown(e, room) {
        // ALLOW dragging in play mode now
        // if (State.mode !== 'edit') return; 

        e.stopPropagation();

        State.isDragging = true;
        State.activeRoomId = room.id;

        // Calculate offset: Mouse Pos relative to Room Origin
        // Room Origin is at (room.x * TILE_SIZE, room.y * TILE_SIZE) relative to MainGrid
        // Mouse Pos is e.clientX/Y

        const gridRect = elements.mainGrid.getBoundingClientRect();
        const roomScreenX = gridRect.left + room.x * TILE_SIZE;
        const roomScreenY = gridRect.top + room.y * TILE_SIZE;

        State.dragOffset.x = e.clientX - roomScreenX;
        State.dragOffset.y = e.clientY - roomScreenY;

        room.element.classList.add('dragging');
    },

    onDragMove(e) {
        if (!State.isDragging || !State.activeRoomId) return;

        const room = State.rooms.find(r => r.id === State.activeRoomId);
        if (!room) return;

        const gridRect = elements.mainGrid.getBoundingClientRect();

        // Desired Room Origin Screen Pos = Mouse Pos - Drag Offset
        // Desired Room Origin Grid Pos = (Desired Screen Pos - Grid Screen Pos) / TILE_SIZE

        const desiredScreenX = e.clientX - State.dragOffset.x;
        const desiredScreenY = e.clientY - State.dragOffset.y;

        let gridX = Math.round((desiredScreenX - gridRect.left) / TILE_SIZE);
        let gridY = Math.round((desiredScreenY - gridRect.top) / TILE_SIZE);

        room.x = gridX;
        room.y = gridY;

        this.updateElementPosition(room.element, gridX, gridY);

        // Check overlap and bounds
        if (this.checkOverlap(room) || this.checkOutOfBounds(room)) {
            room.element.classList.add('invalid');
        } else {
            room.element.classList.remove('invalid');
        }
    },

    onDragEnd(e) {
        if (!State.isDragging) return;

        const room = State.rooms.find(r => r.id === State.activeRoomId);
        if (room) {
            room.element.classList.remove('dragging');
        }

        State.isDragging = false;
        State.activeRoomId = null;
    },

    checkOverlap(targetRoom) {
        const targetTiles = this.getGlobalTiles(targetRoom);

        for (const other of State.rooms) {
            if (other.id === targetRoom.id) continue;

            const otherTiles = this.getGlobalTiles(other);

            for (const t1 of targetTiles) {
                for (const t2 of otherTiles) {
                    if (t1.x === t2.x && t1.y === t2.y) return true;
                }
            }
        }
        return false;
    },

    checkOutOfBounds(room) {
        const tiles = this.getGlobalTiles(room);
        return tiles.some(t => t.x < 0 || t.x >= State.dungeonSize.w || t.y < 0 || t.y >= State.dungeonSize.h);
    },

    getGlobalTiles(room) {
        return room.definition.tiles.map(t => ({
            x: room.x + t.x,
            y: room.y + t.y,
            doors: room.definition.doors.filter(d => d.x === t.x && d.y === t.y)
        }));
    },

    saveLayout() {
        State.savedLayout = State.rooms.map(r => ({
            id: r.id,
            x: r.x,
            y: r.y
        }));
        State.savedPlayerPos = { ...State.playerPos };
        elements.statusText.textContent = "Layout Saved!";
        setTimeout(() => elements.statusText.textContent = "Ready", 2000);
    },

    resetLayout() {
        if (!State.savedLayout) return;

        State.savedLayout.forEach(saved => {
            const room = State.rooms.find(r => r.id === saved.id);
            if (room) {
                room.x = saved.x;
                room.y = saved.y;
                this.updateElementPosition(room.element, room.x, room.y);

                if (this.checkOverlap(room) || this.checkOutOfBounds(room)) {
                    room.element.classList.add('invalid');
                } else {
                    room.element.classList.remove('invalid');
                }
            }
        });

        if (State.playerStart) {
            PlayerController.teleport(State.playerStart.x, State.playerStart.y);
        } else {
            PlayerController.teleport(State.savedPlayerPos.x, State.savedPlayerPos.y);
        }
        elements.statusText.textContent = "Layout Reset!";
    },

    clearAll() {
        elements.mainGrid.innerHTML = '';
        State.rooms = [];
        State.nextRoomId = 1;
        State.playerStart = null; // Clear player start position
        this.renderStartMarker(); // Re-render marker to hide it
    }
};

const PlayerController = {
    init() {
        document.addEventListener('keydown', (e) => this.handleInput(e));
        this.updateVisuals();
    },

    handleInput(e) {
        if (State.mode !== 'play') return;

        let dx = 0;
        let dy = 0;

        switch (e.key) {
            case 'ArrowUp': case 'w': dy = -1; break;
            case 'ArrowDown': case 's': dy = 1; break;
            case 'ArrowLeft': case 'a': dx = -1; break;
            case 'ArrowRight': case 'd': dx = 1; break;
            default: return;
        }

        this.tryMove(dx, dy);
    },

    tryMove(dx, dy) {
        const targetX = State.playerPos.x + dx;
        const targetY = State.playerPos.y + dy;

        // 1. Check if target is valid (exists in any room)
        const currentRoom = this.getRoomAt(State.playerPos.x, State.playerPos.y);
        const targetRoom = this.getRoomAt(targetX, targetY);

        if (!targetRoom) {
            this.shakePlayer();
            return; // Void
        }

        // 2. Check doors if changing rooms
        if (currentRoom !== targetRoom) {
            // Check exit from current
            if (!this.hasDoor(currentRoom, State.playerPos.x, State.playerPos.y, dx, dy)) {
                this.shakePlayer();
                return; // Blocked by wall
            }

            // Optional: Check entry to target?
            // For now, let's assume one-way is enough or just check exit.
        }

        // Move
        State.playerPos.x = targetX;
        State.playerPos.y = targetY;
        this.updateVisuals();
    },

    getRoomAt(x, y) {
        for (const room of State.rooms) {
            const tiles = LayoutManager.getGlobalTiles(room);
            if (tiles.some(t => t.x === x && t.y === y)) return room;
        }
        return null;
    },

    hasDoor(room, globalX, globalY, dx, dy) {
        // Convert global to local
        const localX = globalX - room.x;
        const localY = globalY - room.y;

        // Find door definition
        const door = room.definition.doors.find(d => d.x === localX && d.y === localY);
        if (!door) return false;

        // Check direction
        if (dy === -1 && door.dir === 'top') return true;
        if (dy === 1 && door.dir === 'bottom') return true;
        if (dx === -1 && door.dir === 'left') return true;
        if (dx === 1 && door.dir === 'right') return true;

        return false;
    },

    teleport(x, y) {
        State.playerPos.x = x;
        State.playerPos.y = y;
        this.updateVisuals();
    },

    updateVisuals() {
        elements.playerToken.style.transform = `translate(${State.playerPos.x * TILE_SIZE}px, ${State.playerPos.y * TILE_SIZE}px)`;
        elements.coordsText.textContent = `${State.playerPos.x}, ${State.playerPos.y}`;
    },

    shakePlayer() {
        elements.playerToken.animate([
            { transform: `translate(${State.playerPos.x * TILE_SIZE}px, ${State.playerPos.y * TILE_SIZE}px) translateX(0)` },
            { transform: `translate(${State.playerPos.x * TILE_SIZE}px, ${State.playerPos.y * TILE_SIZE}px) translateX(-5px)` },
            { transform: `translate(${State.playerPos.x * TILE_SIZE}px, ${State.playerPos.y * TILE_SIZE}px) translateX(5px)` },
            { transform: `translate(${State.playerPos.x * TILE_SIZE}px, ${State.playerPos.y * TILE_SIZE}px) translateX(0)` }
        ], {
            duration: 200,
            iterations: 1
        });
    }
};

// Helper for color darkening
function adjustColor(color, amount) {
    return color; // Simplified for now, or implement HSL parsing if needed
}

// --- Initialization ---

function init() {
    RoomCreator.init();
    LayoutManager.init();
    PlayerController.init();

    // Mode Switching
    elements.modeEdit.addEventListener('click', () => setMode('edit'));
    elements.modePlay.addEventListener('click', () => setMode('play'));

    // Drop Zone - Listen on the whole main content area
    const dropZone = document.querySelector('.main-content');
    dropZone.addEventListener('dragover', (e) => e.preventDefault());
    dropZone.addEventListener('drop', (e) => LayoutManager.handleDrop(e));
}

function setMode(mode) {
    State.mode = mode;
    elements.modeEdit.classList.toggle('active', mode === 'edit');
    elements.modePlay.classList.toggle('active', mode === 'play');

    document.body.classList.toggle('mode-play', mode === 'play');

    if (mode === 'play') {
        // Save snapshot for reset
        State.editModeLayout = State.rooms.map(r => ({ id: r.id, x: r.x, y: r.y }));

        elements.playerToken.classList.remove('hidden');
        if (State.playerStart) {
            PlayerController.teleport(State.playerStart.x, State.playerStart.y);
        } else if (!PlayerController.getRoomAt(State.playerPos.x, State.playerPos.y)) {
            if (State.rooms.length > 0) {
                const firstRoom = State.rooms[0];
                const firstTile = firstRoom.definition.tiles[0];
                PlayerController.teleport(firstRoom.x + firstTile.x, firstRoom.y + firstTile.y);
            }
        }
    } else {
        // Restore snapshot
        if (State.editModeLayout) {
            State.editModeLayout.forEach(saved => {
                const room = State.rooms.find(r => r.id === saved.id);
                if (room) {
                    room.x = saved.x;
                    room.y = saved.y;
                    LayoutManager.updateElementPosition(room.element, room.x, room.y);
                    // Re-check validity
                    if (LayoutManager.checkOverlap(room) || LayoutManager.checkOutOfBounds(room)) {
                        room.element.classList.add('invalid');
                    } else {
                        room.element.classList.remove('invalid');
                    }
                }
            });
        }
        elements.playerToken.classList.add('hidden');
    }
}

init();
