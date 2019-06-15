const express = require("express");
const path = require("path");

const distPath = 'client/dist';

const app = express();

app.use(express.static(path.join(__dirname, distPath)));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, distPath, 'index.html'));
});

const port = process.env.PORT || 5001;

app.listen(port, () => console.log(`Web in listening on port ${port}`));
