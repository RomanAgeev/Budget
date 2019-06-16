const express = require("express");
const path = require("path");
const favicon = require("serve-favicon");

const publicPath = path.join(__dirname, './public');

const app = express();

app.use(express.static(publicPath));
app.use(favicon(path.join(__dirname, 'favicon.png')));

app.get('/', (req, res) => {
    res.sendFile(path.join(publicPath, 'index.html'));
});

const port = process.env.PORT || 5001;

app.listen(port, () => console.log(`Web in listening on port ${port}`));
