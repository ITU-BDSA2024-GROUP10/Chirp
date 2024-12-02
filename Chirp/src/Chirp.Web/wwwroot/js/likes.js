async function toggleLike(button, cheepId) {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').getAttribute('content');
    const isLiking = !button.classList.contains('liked');

    const response = await fetch(isLiking ? '/like' : '/unlike', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': csrfToken
        },
        body: `cheepId=${encodeURIComponent(cheepId)}&returnUrl=${encodeURIComponent(window.location.pathname)}`
    });

    if (response.ok) {
        const data = await response.json();
        button.innerText = `Like (${data.likeCount})`;
        button.classList.toggle('liked', isLiking);
    } else {
        alert('Failed to update like status');
    }
}