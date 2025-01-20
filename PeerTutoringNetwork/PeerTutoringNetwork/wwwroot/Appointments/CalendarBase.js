export class CalendarBase {
    // TODO singleton CalendarBase
    static instance;

    constructor(selector, events) {
        if (CalendarBase.instance) {
            return CalendarBase.instance;
        }
        this.el = document.querySelector(selector);
        this.events = events;
        this.current = moment().date(1);
        this.draw();
        const current = document.querySelector('.today');
        if (current) {
            setTimeout(() => {
                this.openDay(current);
            }, 500);
        }
        CalendarBase.instance = this;
    }

    draw() {
        this.drawHeader();
        this.drawMonth();
    }

    drawHeader() {
        if (!this.header) {
            this.header = this.createElement('div', 'header');
            this.title = this.createElement('h1');
            const right = this.createElement('div', 'right');
            right.addEventListener('click', () => this.nextMonth());
            const left = this.createElement('div', 'left');
            left.addEventListener('click', () => this.prevMonth());
            this.header.appendChild(this.title);
            this.header.appendChild(right);
            this.header.appendChild(left);
            this.el.appendChild(this.header);
        }
        this.title.innerHTML = this.current.format('MMMM YYYY');
    }

    drawMonth() {
        this.events.forEach(ev => {
            ev.date = new moment(ev.date);
        });

        if (this.month) {
            this.oldMonth = this.month;
            this.oldMonth.className = 'month out ' + (this.next ? 'next' : 'prev');
            this.oldMonth.addEventListener('webkitAnimationEnd', () => {
                this.oldMonth.parentNode.removeChild(this.oldMonth);
                this.month = this.createElement('div', 'month');
                this.backFill();
                this.currentMonth();
                this.forwardFill();
                this.el.appendChild(this.month);
                setTimeout(() => {
                    this.month.className = 'month in ' + (this.next ? 'next' : 'prev');
                }, 16);
            });
        } else {
            this.month = this.createElement('div', 'month');
            this.el.appendChild(this.month);
            this.backFill();
            this.currentMonth();
            this.forwardFill();
            this.month.className = 'month new';
        }
    }

    backFill() {
        const clone = this.current.clone();
        const dayOfWeek = clone.day();
        if (!dayOfWeek) return;
        clone.subtract('days', dayOfWeek + 1);
        for (let i = dayOfWeek; i > 0; i--) {
            this.drawDay(clone.add('days', 1));
        }
    }

    forwardFill() {
        const clone = this.current.clone().add('months', 1).subtract('days', 1);
        const dayOfWeek = clone.day();
        if (dayOfWeek === 6) return;
        for (let i = dayOfWeek; i < 6; i++) {
            this.drawDay(clone.add('days', 1));
        }
    }

    currentMonth() {
        const clone = this.current.clone();
        while (clone.month() === this.current.month()) {
            this.drawDay(clone);
            clone.add('days', 1);
        }
    }

    getWeek(day) {
        if (!this.week || day.day() === 0) {
            this.week = this.createElement('div', 'week');
            this.month.appendChild(this.week);
        }
    }

    drawDay(day) {
        this.getWeek(day);
        const outer = this.createElement('div', this.getDayClass(day));
        outer.addEventListener('click', () => {
            document.querySelectorAll('.day').forEach(d => d.classList.remove('selected'));
            outer.classList.add('selected');
            this.openDay(outer);
        });
        const name = this.createElement('div', 'day-name', day.format('ddd'));
        const number = this.createElement('div', 'day-number', day.format('DD'));
        const events = this.createElement('div', 'day-events');
        this.drawEvents(day, events);
        outer.appendChild(name);
        outer.appendChild(number);
        outer.appendChild(events);
        this.week.appendChild(outer);
    }

    drawEvents(day, element) {
        if (day.month() === this.current.month()) {
            const todaysEvents = this.events.reduce((memo, ev) => {
                if (ev.date.isSame(day, 'day')) {
                    memo.push(ev);
                }
                return memo;
            }, []);
            todaysEvents.forEach(ev => {
                const evSpan = this.createElement('span', ev.color);
                element.appendChild(evSpan);
            });
        }
    }

    getDayClass(day) {
        const classes = ['day'];
        if (day.month() !== this.current.month()) {
            classes.push('other');
        } else if (moment().isSame(day, 'day')) {
            classes.push('today');
        }
        return classes.join(' ');
    }

    // TODO ocp 
    openDay(el) {
        // To be implemented by subclasses
    }

    createElement(tagName, className, innerText) {
        const ele = document.createElement(tagName);
        if (className) {
            ele.className = className;
        }
        if (innerText) {
            ele.innerText = ele.textContent = innerText;
        }
        return ele;
    }

    nextMonth() {
        this.current.add('months', 1);
        this.next = true;
        this.draw();
    }

    prevMonth() {
        this.current.subtract('months', 1);
        this.next = false;
        this.draw();
    }
}