<template>
    <div class="container">
        <h1>URL Shortener</h1>
        <div class="input-section">
            <input v-model="originalUrl" placeholder="Enter URL" @keyup.enter="shortenUrl" />
            <button @click="shortenUrl">Shorten</button>
        </div>
        <div v-if="shortenedUrl" class="result">
            Shortened URL: <a :href="shortenedUrl" target="_blank">{{ shortenedUrl }}</a>
        </div>
        <!-- Optional: Add a link to Swagger -->
        <div class="backend-link">
            <a href="http://localhost:5281/swagger/index.html" target="_blank">View API Documentation</a>
        </div>
        <h2>Shortened URLs</h2>
        <ul>
            <li v-for="url in urls" :key="url.id">
                <div class="url-group">
                    <a :href="`/${url.shortCode}`" @click.prevent="redirect(url.shortCode)">
                        {{ `http://localhost:8080/${url.shortCode}` }}
                    </a>
                    <a :href="url.originalUrl" target="_blank" class="original-url">
                        {{ url.originalUrl }}
                    </a>
                </div>
                <button class="delete-btn" @click="deleteUrl(url.shortCode)">Delete</button>
            </li>
        </ul>
    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        data() {
            return {
                originalUrl: '',
                shortenedUrl: '',
                urls: []
            };
        },
        mounted() {
            this.fetchUrls();
        },
        methods: {
            async shortenUrl() {
                try {
                    const response = await axios.post('/api/urlshortener', this.originalUrl, {
                        headers: { 'Content-Type': 'application/json' }
                    });
                    this.shortenedUrl = response.data.shortUrl;
                    this.originalUrl = '';
                    this.fetchUrls();
                } catch (error) {
                    console.error('Shorten URL Error:', error.message);
                    alert('Error shortening URL: ' + error.message);
                }
            },
            async fetchUrls() {
                try {
                    const response = await axios.get('/api/urlshortener');
                    this.urls = response.data;
                } catch (error) {
                    console.error('Fetch URLs Error:', error.message);
                }
            },
            async redirect(shortCode) {
                // This still redirects to the short URL, not Swagger
                window.location.href = `/${shortCode}`;
            },
            async deleteUrl(shortCode) {
                try {
                    const url = `/api/urlshortener/${shortCode}`;
                    console.log('Deleting URL:', url);
                    await axios.delete(url);
                    this.fetchUrls();
                } catch (error) {
                    console.error('Delete URL Error:', error.message);
                    alert('Error deleting URL: ' + error.message);
                }
            }
        }
    };
</script>

<style>
    .container {
        max-width: 800px;
        margin: 0 auto;
        padding: 20px;
    }

    .input-section {
        display: flex;
        gap: 10px;
        margin-bottom: 20px;
    }

    input {
        flex: 1;
        padding: 8px;
    }

    button {
        padding: 8px 16px;
    }

    .result {
        margin-bottom: 20px;
    }

    .backend-link {
        margin-bottom: 20px;
    }

    ul {
        list-style: none;
        padding: 0;
    }

    li {
        margin: 10px 0;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .url-group {
        display: flex;
        gap: 10px;
        align-items: center;
    }

    .original-url {
        color: #555;
        text-decoration: none;
    }

        .original-url:hover {
            text-decoration: underline;
        }

    .delete-btn {
        background-color: #ff4444;
        color: white;
        border: none;
        padding: 5px 10px;
        cursor: pointer;
    }

        .delete-btn:hover {
            background-color: #cc0000;
        }
</style>