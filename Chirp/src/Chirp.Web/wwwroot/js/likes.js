async function toggleLike(button, cheepId, isLiking) {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').getAttribute('content');

    try {
        const response = await fetch('/like', {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': csrfToken 
            },
            body: `cheepId=${encodeURIComponent(cheepId)}&isLiking=${isLiking}&returnUrl=${encodeURIComponent(window.location.pathname)}`
        });

        if (response.ok) {
            const newIsLiking = !isLiking;
            button.innerText = newIsLiking ? `Unlike` : `Like`;
            button.setAttribute('onclick', `toggleLike(this, ${cheepId}, ${newIsLiking})`);
        } else {
            alert('Failed to update like status');
        }
    } catch (error) {
        console.error('Failed to toggle like:', error);
    }
}