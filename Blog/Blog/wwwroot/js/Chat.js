var renameRoomBtn = document.getElementById('rename-room-btn')
var renameRoomModal = document.getElementById('rename-room-modal')
renameRoomBtn.addEventListener('click', function () {
    renameRoomModal.classList.remove('modal')
    renameRoomModal.classList.add('chat-modal-active')
    event.preventDefault();
})

function closeRenameModal() {
    renameRoomModal.classList.remove('chat-modal-active')
    renameRoomModal.classList.add('modal')
}

var leaveRoomBtn = document.getElementById('leave-room-btn')
var leaveRoomModal = document.getElementById('leave-room-modal')
leaveRoomBtn.addEventListener('click', function () {
    leaveRoomModal.classList.remove('modal')
    leaveRoomModal.classList.add('chat-modal-active')
    event.preventDefault();
})

function closeLeaveModal() {
    leaveRoomModal.classList.remove('chat-modal-active')
    leaveRoomModal.classList.add('modal')
}