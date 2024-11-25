async function toggleFollow(button, followName, shouldFollow) {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').getAttribute('content');

    const response = await fetch('/follow', {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': csrfToken 
        },
        body: `followName=${encodeURIComponent(followName)}&shouldFollow=${shouldFollow}&returnUrl=${encodeURIComponent(window.location.pathname)}`
    });

    if (response.ok) {
        const buttons = document.querySelectorAll(`button[onclick*="'${followName}'"]`);
        buttons.forEach(btn => {
            btn.innerText = shouldFollow ? 'unfollow' : 'follow';
            btn.setAttribute('onclick', `toggleFollow(this, '${followName}', ${!shouldFollow})`);
        });
    } else {
        alert('Failed to update follow status');
    }
}