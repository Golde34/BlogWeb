var createRoomBtn = document.getElementById('create-room-btn')
var createRoomModal = document.getElementById('create-room-modal')
createRoomBtn.addEventListener('click', function () {
    createRoomModal.classList.add('modal-active')
    event.preventDefault();
})

function closeModal() {
    createRoomModal.classList.remove('modal-active')
}

var notiBtn = document.getElementById('notification-btn')
var notiModal = document.getElementById('notification-modal')
notiBtn.addEventListener('click', function () {
    notiModal.classList.remove('modal')
    notiModal.classList.add('chat-modal-active')
    event.preventDefault();
})

function closeNotiModal() {
    notiModal.classList.remove('chat-modal-active')
    notiModal.classList.add('modal')
}