async function toggleFollow(button, followName, shouldFollow) {
    console.log(`Sending: followName=${followName}, shouldFollow=${shouldFollow}, returnUrl=${window.location.pathname}`);
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
        button.innerText = shouldFollow ? 'unfollow' : 'follow';
        button.setAttribute('onclick', `toggleFollow(this, '${followName}', ${!shouldFollow})`);
    } else {
        alert('Failed to update follow status');
    }
}