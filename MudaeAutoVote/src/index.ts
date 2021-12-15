import puppeteer from 'puppeteer'
import {join} from 'path'
import { config } from 'dotenv';
config()
let t = process.env.tok as string;
(async()=>{
    let ext = join(__dirname,"../ext/uBlock0.chromium/")
    const browser = await puppeteer.launch({
        devtools:true,
        headless:false,
        args:[ `--load-extension=${ext}`, `--disable-extensions-except=${ext}`]
    })
    // add auth
    const page = (await browser.pages())[0]
    await page.setViewport({
        width: 1280,
        height: 800,
        isMobile: false,
    });
    await page.setCookie({
        url:"https://top.gg/",
        name:"betaOptIn",
        value:"true"
    })
    console.log("Setup beta cookie, Going to discord")
    await page.goto("https://discord.com/login", {
        waitUntil: "networkidle2"
    });
    console.log("forcing login...")
    await page.evaluate((mytoken)=>{
        console.log(mytoken)
        function gamer(lmao:string) {
            setInterval(() => {
                // @ts-ignore
                document.body.appendChild(document.createElement `iframe`).contentWindow.localStorage.token = `"${lmao}"`
            }, 50);
            setTimeout(() => {
                location.reload();
            }, 2500);
        }
        gamer(mytoken);
    }, t);
    let nav = await page.waitForNavigation()
    console.log("brute force login done.",nav?.url())
    await page.goto('https://discord.com/oauth2/authorize?client_id=264434993625956352&scope=identify%20guilds%20email&state=L2JvdC80MzI2MTAyOTIzNDI1ODczOTIvdm90ZQ==&redirect_uri=https%3A%2F%2Ftop.gg%2Flogin%2Fcallback&response_type=code');
    await page.waitForSelector("#app-mount > div.app-1q1i1E > div > div > div > div > div.footer-3ZalXG > button.button-38aScr.lookFilled-1Gx00P.colorBrand-3pXr91.sizeMedium-1AC_Sl.grow-q77ONN")
    await page.click("#app-mount > div.app-1q1i1E > div > div > div > div > div.footer-3ZalXG > button.button-38aScr.lookFilled-1Gx00P.colorBrand-3pXr91.sizeMedium-1AC_Sl.grow-q77ONN")
    // allow for new ui design
    await page.setViewport({
        width: 1920,
        height: 1080,
        isMobile: false,
    });

    await page.waitForNavigation()
    await (page.click("#modal-root > div.modal-content.content > div > a:nth-child(4)").catch(()=>console.log("no ad")))
    await page.click("#__next > div > div.page__PageContentWrapper-iv577v-0.jVUzBL > div > div > div.css-d171a1 > div > div.css-1ilyui9 > main > div.css-u9cil6 > div > div.css-1yn6pjb > button").catch(()=>console.log("failure"))
    console.log("clicked button?")
    await page.waitForTimeout(2e3)
    let e1 = await page.$('#__next > div > div.page__PageContentWrapper-iv577v-0.jVUzBL > div > div > div.css-d171a1 > div > div.css-1ilyui9 > main > div.css-u9cil6 > div > div.css-j4hctk > div > p.chakra-text.css-1apiyub').catch(()=>console.log("ok."))
    if (e1) {
        let value = await page.evaluate(el => el.textContent, e1)
        if (value.includes("You have already voted"))
            return console.log("failure _AV"), await browser.close();
    }
    let element = await page.$('#__next > div > div.page__PageContentWrapper-iv577v-0.jVUzBL > div > div > div.css-d171a1 > div > div.css-1ilyui9 > main > div.css-u9cil6 > div > div.css-rnfiu7 > div > p.chakra-text.css-190ynl8').catch(()=>console.log("failure _AV2"))
    if (!element)
        return browser.close();
    let value = await page.evaluate(el => el.textContent, element)
    if (value.includes("Thanks ")) {
        console.log("Success")
    }
    else
        console.log("probably fine?")

    await browser.close()
})()