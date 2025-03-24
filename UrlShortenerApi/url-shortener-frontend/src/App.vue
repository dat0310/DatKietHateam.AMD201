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
        <h2>Shortened URLs</h2>
        <ul>
            <li v-for="url in urls" :key="url.id">
                <a :href="url.shortCode" @click.prevent="redirect(url.shortCode)">
                    {{ `http://localhost:5281/${url.shortCode}` }}
                </a> - {{ url.originalUrl }}
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
                    const response = await axios.post('http://localhost:5281/api/urlshortener', this.originalUrl, {
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
                    const response = await axios.get('http://localhost:5281/api/urlshortener');
                    this.urls = response.data;
                } catch (error) {
                    console.error('Fetch URLs Error:', error.message);
                }
            },
            async redirect(shortCode) {
                window.location.href = `http://localhost:5281/${shortCode}`;
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

    ul {
        list-style: none;
        padding: 0;
    }

    li {
        margin: 10px 0;
    }
</style>